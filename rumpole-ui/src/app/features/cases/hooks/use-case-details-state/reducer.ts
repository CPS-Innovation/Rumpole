import { ApiResult } from "../../../../common/types/ApiResult";
import { resolvePdfUrl } from "../../api/gateway-api";
import { CaseDocumentWithUrl } from "../../domain/CaseDocumentWithUrl";
import { mapAccordionState } from "./map-accordion-state";
import { CombinedState } from "../../domain/CombinedState";
import { CaseDetails } from "../../domain/CaseDetails";
import { CaseDocument } from "../../domain/CaseDocument";
import { PipelineResults } from "../../domain/PipelineResults";
import { ApiTextSearchResult } from "../../domain/ApiTextSearchResult";
import { mapTextSearch } from "./map-text-search";
import { mapMissingDocuments } from "./map-missing-documents";
import { sortMappedTextSearchResult } from "./sort-mapped-text-search-result";
import { mapDocumentsState } from "./map-documents-state";
import { mapFilters } from "./map-filters";
import { MappedDocumentResult } from "../../domain/MappedDocumentResult";
import { isDocumentVisible } from "./is-document-visible";
import { AsyncPipelineResult } from "../use-pipeline-api/AsyncPipelineResult";

export const reducer = (
  state: CombinedState,
  action:
    | { type: "UPDATE_CASE_DETAILS"; payload: ApiResult<CaseDetails> }
    | {
        type: "UPDATE_CASE_DOCUMENTS";
        payload: ApiResult<CaseDocument[]>;
      }
    | {
        type: "UPDATE_PIPELINE";
        payload: AsyncPipelineResult<PipelineResults>;
      }
    | {
        type: "OPEN_PDF";
        payload: { tabSafeId: string; pdfId: string };
      }
    | {
        type: "CLOSE_PDF";
        payload: { tabSafeId: string };
      }
    | {
        type: "UPDATE_AUTH_TOKEN";
        payload: ApiResult<Headers>;
      }
    | {
        type: "UPDATE_SEARCH_TERM";
        payload: { searchTerm: string };
      }
    | {
        type: "LAUNCH_SEARCH_RESULTS";
      }
    | {
        type: "UPDATE_SEARCH_RESULTS";
        payload: ApiResult<undefined | ApiTextSearchResult[]>;
      }
    | {
        type: "CLOSE_SEARCH_RESULTS";
      }
    | {
        type: "CHANGE_RESULTS_ORDER";
        payload: CombinedState["searchState"]["resultsOrder"];
      }
    | {
        type: "UPDATE_FILTER";
        payload: {
          filter: keyof CombinedState["searchState"]["filterOptions"];
          id: string;
          isSelected: boolean;
        };
      }
): CombinedState => {
  switch (action.type) {
    case "UPDATE_CASE_DETAILS":
      if (action.payload.status === "failed") {
        throw action.payload.error;
      }

      return { ...state, caseState: action.payload };
    case "UPDATE_CASE_DOCUMENTS":
      if (action.payload.status === "failed") {
        throw action.payload.error;
      }
      const documentsState = mapDocumentsState(action.payload);

      const accordionState = mapAccordionState(documentsState);

      return {
        ...state,
        documentsState,
        accordionState,
      };
    case "UPDATE_PIPELINE":
      if (action.payload.status === "failed") {
        throw action.payload.error;
      }

      if (action.payload.status === "initiating") {
        return state;
      }

      const newPipelineResults = action.payload;

      const coreNextPipelineState = {
        ...state,
        pipelineState: {
          ...newPipelineResults,
        },
      };

      const openPdfsWeNeedToUpdate = newPipelineResults.data.documents
        .filter(
          (item) =>
            item.pdfBlobName &&
            state.tabsState.items.some(
              (tabItem) =>
                tabItem.documentId === item.documentId && !tabItem.url
            )
        )
        .map(({ documentId, pdfBlobName }) => ({ documentId, pdfBlobName }));

      if (!openPdfsWeNeedToUpdate.length) {
        return coreNextPipelineState;
      }

      const nextOpenTabs = state.tabsState.items.reduce((prev, curr) => {
        const matchingFreshPdfRecord = openPdfsWeNeedToUpdate.find(
          (item) => item.documentId === curr.documentId
        );

        if (matchingFreshPdfRecord) {
          const url = resolvePdfUrl(matchingFreshPdfRecord.pdfBlobName);
          return [...prev, { ...curr, url }];
        }
        return [...prev, curr];
      }, [] as CaseDocumentWithUrl[]);

      return {
        ...coreNextPipelineState,
        tabsState: { ...state.tabsState, items: nextOpenTabs },
      };

    case "OPEN_PDF":
      const { pdfId, tabSafeId } = action.payload;

      const coreNewState = {
        ...state,
        searchState: {
          ...state.searchState,
          isResultsVisible: false,
        },
      };

      if (
        !state.tabsState.items.some((item) => item.documentId === pdfId) &&
        state.documentsState.status === "succeeded"
      ) {
        const foundDocument = state.documentsState.data.find(
          (item) => item.documentId === pdfId
        )!;
        const blobName = state.pipelineState.haveData
          ? state.pipelineState.data.documents.find(
              (item) => item.documentId === pdfId
            )?.pdfBlobName
          : undefined;

        const url = blobName && resolvePdfUrl(blobName);

        return {
          ...coreNewState,
          tabsState: {
            ...state.tabsState,
            items: [
              ...state.tabsState.items,
              { ...foundDocument, url, tabSafeId },
            ],
          },
          searchState: {
            ...state.searchState,
            isResultsVisible: false,
          },
        };
      }

      return coreNewState;
    case "CLOSE_PDF": {
      const { tabSafeId } = action.payload;

      return {
        ...state,
        tabsState: {
          ...state.tabsState,
          items: state.tabsState.items.filter(
            (item) => item.tabSafeId !== tabSafeId
          ),
        },
      };
    }
    case "UPDATE_AUTH_TOKEN":
      if (action.payload.status === "failed") {
        throw action.payload.error;
      }

      if (action.payload.status === "succeeded") {
        const authToken = action.payload.data.get("Authorization") || undefined;
        return { ...state, tabsState: { ...state.tabsState, authToken } };
      }

      return state;
    case "UPDATE_SEARCH_TERM":
      return {
        ...state,
        searchTerm: action.payload.searchTerm,
      };
    case "CLOSE_SEARCH_RESULTS":
      return {
        ...state,
        searchState: {
          ...state.searchState,
          isResultsVisible: false,
        },
      };
    case "LAUNCH_SEARCH_RESULTS":
      return {
        ...state,
        searchState: {
          ...state.searchState,
          isResultsVisible: true,
          submittedSearchTerm: state.searchTerm,
        },
      };

    case "UPDATE_SEARCH_RESULTS":
      if (action.payload.status === "failed") {
        throw action.payload.error;
      }

      if (action.payload.status === "loading") {
        return {
          ...state,
          searchState: {
            ...state.searchState,
            results: { status: "loading" },
          },
        };
      }

      if (
        state.documentsState.status === "succeeded" &&
        state.pipelineState.status === "complete" &&
        state.searchState.submittedSearchTerm &&
        action.payload.data
      ) {
        const unsortedData = mapTextSearch(
          action.payload.data,
          state.documentsState.data,
          state.searchState.submittedSearchTerm
        );

        const sortedData = sortMappedTextSearchResult(
          unsortedData,
          state.searchState.resultsOrder
        );

        const missingDocs = mapMissingDocuments(
          state.pipelineState.data,
          state.documentsState.data
        );

        const filterOptions = mapFilters(unsortedData);

        return {
          ...state,
          searchState: {
            ...state.searchState,
            missingDocs,
            filterOptions,
            results: {
              status: "succeeded",
              data: sortedData,
            },
          },
        };
      }

      return state;

    case "CHANGE_RESULTS_ORDER":
      return {
        ...state,
        searchState: {
          ...state.searchState,
          resultsOrder: action.payload,
          results:
            state.searchState.results.status === "loading"
              ? // if loading, then there are no stable results to search,
                //  also required for type checking :)
                state.searchState.results
              : {
                  ...state.searchState.results,
                  data: sortMappedTextSearchResult(
                    state.searchState.results.data,
                    action.payload
                  ),
                },
        },
      };

    case "UPDATE_FILTER":
      const { isSelected, filter, id } = action.payload;

      const nextState = {
        ...state,
        searchState: {
          ...state.searchState,
          filterOptions: {
            ...state.searchState.filterOptions,
            [filter]: {
              ...state.searchState.filterOptions[filter],
              [id]: {
                ...state.searchState.filterOptions[filter][id],
                isSelected,
              },
            },
          },
        },
      };

      if (state.searchState.results.status !== "succeeded") {
        return nextState;
      }

      const nextResults = state.searchState.results.data.documentResults.reduce(
        (acc, curr) => {
          const { isVisible, hasChanged } = isDocumentVisible(
            curr,
            nextState.searchState.filterOptions
          );

          acc.push(hasChanged ? { ...curr, isVisible } : curr);
          return acc;
        },
        [] as MappedDocumentResult[]
      );

      const { filteredDocumentCount, filteredOccurrencesCount } =
        nextResults.reduce(
          (acc, curr) => {
            if (curr.isVisible) {
              acc.filteredDocumentCount += 1;
              acc.filteredOccurrencesCount += curr.occurrencesInDocumentCount;
            }

            return acc;
          },
          { filteredDocumentCount: 0, filteredOccurrencesCount: 0 }
        );

      return {
        ...nextState,
        searchState: {
          ...nextState.searchState,
          results: {
            ...state.searchState.results,
            data: {
              ...state.searchState.results.data,
              documentResults: nextResults,
              filteredDocumentCount,
              filteredOccurrencesCount,
            },
          },
        },
      };

    default:
      throw new Error("Unknown action passed to case details reducer");
  }
};

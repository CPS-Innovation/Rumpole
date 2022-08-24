import { ApiResult } from "../../../../common/types/ApiResult";
import { resolvePdfUrl } from "../../api/gateway-api";
import { CaseDocumentViewModel } from "../../domain/CaseDocumentViewModel";
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
import { mapHighlights } from "./map-highlights";
import { NewPdfHighlight } from "../../domain/NewPdfHighlight";

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
        payload: {
          tabSafeId: string;
          pdfId: string;
          mode: CaseDocumentViewModel["mode"];
        };
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
    | {
        type: "ADD_REDACTION";
        payload: {
          pdfId: string;
          redaction: NewPdfHighlight;
        };
      }
    | {
        type: "REMOVE_REDACTION";
        payload: {
          pdfId: string;
          redactionId: string;
        };
      }
    | {
        type: "REMOVE_ALL_REDACTIONS";
        payload: {
          pdfId: string;
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

      /*
        Note: if we are looking for open tabs that do not yet know their url (i.e. the
          user has opened a document from the accordion before the pipeline has given us
          the blob name for that document), it can only be after the document has been
          launched from the accordion.  This means we don't have to worry about search
          highlighting from this point on, only setting the URL (i.e. the document will be in 
          "read" mode, not "search" mode)
      */

      const nextOpenTabs = state.tabsState.items.reduce((prev, curr) => {
        const matchingFreshPdfRecord = openPdfsWeNeedToUpdate.find(
          (item) => item.documentId === curr.documentId
        );

        if (matchingFreshPdfRecord) {
          const url = resolvePdfUrl(matchingFreshPdfRecord.pdfBlobName);
          return [...prev, { ...curr, url }];
        }
        return [...prev, curr];
      }, [] as CaseDocumentViewModel[]);

      return {
        ...coreNextPipelineState,
        tabsState: { ...state.tabsState, items: nextOpenTabs },
      };

    case "OPEN_PDF":
      const { pdfId, tabSafeId, mode } = action.payload;

      const coreNewState = {
        ...state,
        searchState: {
          ...state.searchState,
          isResultsVisible: false,
        },
      };

      if (state.documentsState.status !== "succeeded") {
        // this is just here to keep typing happy, it is not logically
        //  possible to be opening a pdf without the documents call
        //  having already completed.
        return coreNewState;
      }

      const tabAlreadyOpenedInRequiredState = state.tabsState.items.some(
        (item) =>
          item.documentId === pdfId &&
          // we have found the tab already exists in read mode and we are trying to
          //  open again in read mode
          ((item.mode === "read" && mode === "read") ||
            // we have found the tab open in search mode and we are trying to open again
            //  with the exact same search term
            (item.mode === "search" &&
              mode === "search" &&
              item.searchTerm === state.searchState.submittedSearchTerm))
      );

      if (tabAlreadyOpenedInRequiredState) {
        // there is nothing more to do, the tab control will show the appropriate tab
        //  via the url hash functionality
        return coreNewState;
      }
      const alreadyOpenedTabIndex = state.tabsState.items.findIndex(
        (item) => item.documentId === pdfId
      );

      const redactionsHighlightsToRetain =
        alreadyOpenedTabIndex !== -1
          ? state.tabsState.items[alreadyOpenedTabIndex].redactionHighlights
          : [];

      const foundDocument = state.documentsState.data.find(
        (item) => item.documentId === pdfId
      )!;

      const blobName = state.pipelineState.haveData
        ? state.pipelineState.data.documents.find(
            (item) => item.documentId === pdfId
          )?.pdfBlobName
        : undefined;

      const url = blobName && resolvePdfUrl(blobName);

      let item: CaseDocumentViewModel;

      const coreItem = {
        ...foundDocument,
        url,
        tabSafeId,
        redactionHighlights: redactionsHighlightsToRetain,
      };

      if (mode === "read") {
        item = {
          ...coreItem,
          mode: "read",
        };
      } else {
        const foundDocumentSearchResult =
          state.searchState.results.status === "succeeded" &&
          state.searchState.results.data.documentResults.find(
            (item) => item.documentId === pdfId
          )!;

        const pageOccurrences = foundDocumentSearchResult
          ? foundDocumentSearchResult.occurrences.reduce((acc, curr) => {
              let foundPage = acc.find(
                (item) => item.pageIndex === curr.pageIndex
              );

              if (!foundPage) {
                foundPage = {
                  pageIndex: curr.pageIndex,
                  boundingBoxes: [],
                };
                acc.push(foundPage);
              }

              foundPage.boundingBoxes = [
                ...foundPage.boundingBoxes,
                ...curr.occurrencesInLine,
              ];

              return acc;
            }, [] as { pageIndex: number; boundingBoxes: number[][] }[])
          : /* istanbul ignore next */ [];

        const searchHighlights = mapHighlights(pageOccurrences);

        item = {
          ...coreItem,
          mode: "search",
          searchTerm: state.searchState.submittedSearchTerm!,
          occurrencesInDocumentCount: foundDocumentSearchResult
            ? foundDocumentSearchResult.occurrencesInDocumentCount
            : /* istanbul ignore next */ 0,
          searchHighlights,
        };
      }

      const nextItemsArray =
        alreadyOpenedTabIndex === -1
          ? // this is the first time we are opening this tab
            [...state.tabsState.items, item]
          : // this is a subsequent time, and the tab is now different (maybe going from
            //  read to search mode or maybe a different search term)
            state.tabsState.items.map((existingItem, index) =>
              index === alreadyOpenedTabIndex
                ? { ...existingItem, ...item }
                : existingItem
            );

      return {
        ...coreNewState,
        tabsState: {
          ...state.tabsState,
          items: nextItemsArray,
        },
        searchState: {
          ...state.searchState,
          isResultsVisible: false,
        },
      };

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
    case "ADD_REDACTION": {
      const { pdfId, redaction } = action.payload;

      return {
        ...state,
        tabsState: {
          ...state.tabsState,
          items: state.tabsState.items.map((item) =>
            item.documentId === pdfId
              ? {
                  ...item,
                  redactionHighlights: [
                    ...item.redactionHighlights,
                    { ...redaction, id: String(+new Date()) },
                  ],
                }
              : item
          ),
        },
      };
    }
    case "REMOVE_REDACTION": {
      const { redactionId, pdfId } = action.payload;

      return {
        ...state,
        tabsState: {
          ...state.tabsState,
          items: state.tabsState.items.map((item) =>
            item.documentId === pdfId
              ? {
                  ...item,
                  redactionHighlights: item.redactionHighlights.filter(
                    (redaction) => redaction.id !== redactionId
                  ),
                }
              : item
          ),
        },
      };
    }

    case "REMOVE_ALL_REDACTIONS": {
      const { pdfId } = action.payload;

      return {
        ...state,
        tabsState: {
          ...state.tabsState,
          items: state.tabsState.items.map((item) =>
            item.documentId === pdfId
              ? {
                  ...item,
                  redactionHighlights: [],
                }
              : item
          ),
        },
      };
    }
    default:
      throw new Error("Unknown action passed to case details reducer");
  }
};

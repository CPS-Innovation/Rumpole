import { useCallback, useEffect } from "react";
import { useApi } from "../../../../common/hooks/useApi";
import {
  getCaseDetails,
  getCaseDocumentsList,
  getHeaders,
} from "../../api/gateway-api";
import { usePipelineApi } from "../use-pipeline-api/usePipelineApi";
import { CombinedState } from "../../domain/CombinedState";
import { reducer } from "./reducer";
import { searchCaseWhenReady } from "./search-case-when-ready";
import { CaseDocumentViewModel } from "../../domain/CaseDocumentViewModel";
import { NewPdfHighlight } from "../../domain/NewPdfHighlight";
import { useReducerAsync } from "use-reducer-async";
import { reducerAsyncActionHandlers } from "./reducer-async-action-handlers";

export type CaseDetailsState = ReturnType<typeof useCaseDetailsState>;

export const initialState = {
  caseState: { status: "loading" },
  documentsState: { status: "loading" },
  pipelineState: { status: "initiating", haveData: false },
  accordionState: { status: "loading" },
  tabsState: { items: [], authToken: undefined },
  searchTerm: "",
  searchState: {
    isResultsVisible: false,
    submittedSearchTerm: undefined,
    resultsOrder: "byDateDesc",
    filterOptions: {
      docType: {},
      category: {},
    },
    missingDocs: [],
    results: { status: "loading" },
  },
} as Omit<CombinedState, "caseId">;

export const useCaseDetailsState = (id: string) => {
  const caseState = useApi(getCaseDetails, id);
  const documentsState = useApi(getCaseDocumentsList, id);
  const pipelineState = usePipelineApi(id);
  const headers = useApi(getHeaders);

  const [combinedState, dispatch] = useReducerAsync(
    reducer,
    { ...initialState, caseId: id },
    reducerAsyncActionHandlers
  );

  useEffect(
    () => dispatch({ type: "UPDATE_CASE_DETAILS", payload: caseState }),
    [caseState, dispatch]
  );

  useEffect(
    () => dispatch({ type: "UPDATE_CASE_DOCUMENTS", payload: documentsState }),
    [documentsState, dispatch]
  );

  useEffect(
    () => dispatch({ type: "UPDATE_PIPELINE", payload: pipelineState }),
    [pipelineState, dispatch]
  );

  useEffect(
    () => dispatch({ type: "UPDATE_AUTH_TOKEN", payload: headers }),
    [headers, dispatch]
  );

  const searchResults = useApi(
    searchCaseWhenReady,
    id,
    combinedState.searchState.submittedSearchTerm,
    //  Note: we let the user trigger a search without the pipeline being ready.
    //  If we additionally observe the complete-state of the pipeline here, we can ensure that a search
    //  is triggered when either:
    //  a) the pipeline is ready and the user subsequently submits a search
    //  b) the user submits a search before the pipeline is ready, but it then becomes ready
    combinedState.pipelineState.status === "complete",
    //  It makes it much easier if we enforce that the documents need to be known before allowing
    //   a search (logically, we do not need to wait for the documents call to return at the point we trigger a
    //   search, we only need them when we map the eventual result of the search call).  However, this is a tidier
    //   place to enforce the wait as we are already waiting for the pipeline here. If we don't wait here, then
    //   we have to deal with the condition where the search results have come back but we do not yet have the
    //   the documents result, and we have to chase up fixing the full mapped objects at that later point.
    //   (Assumption: this is edge-casey stuff as the documents call should always really have come back unless
    //   the user is super quick to trigger a search).
    combinedState.documentsState.status === "succeeded"
  );

  useEffect(
    () => dispatch({ type: "UPDATE_SEARCH_RESULTS", payload: searchResults }),
    [searchResults, dispatch]
  );

  const handleOpenPdf = useCallback(
    (caseDocument: {
      tabSafeId: string;
      documentId: string;
      mode: CaseDocumentViewModel["mode"];
    }) => {
      dispatch({
        type: "OPEN_PDF",
        payload: {
          pdfId: caseDocument.documentId,
          tabSafeId: caseDocument.tabSafeId,
          mode: caseDocument.mode,
        },
      });
    },
    [dispatch]
  );

  const handleClosePdf = useCallback(
    (caseDocument: { tabSafeId: string }) => {
      dispatch({
        type: "CLOSE_PDF",
        payload: {
          tabSafeId: caseDocument.tabSafeId,
        },
      });
    },
    [dispatch]
  );

  const handleSearchTermChange = useCallback(
    (searchTerm: string) => {
      dispatch({
        type: "UPDATE_SEARCH_TERM",
        payload: {
          searchTerm,
        },
      });
    },
    [dispatch]
  );

  const handleLaunchSearchResults = useCallback(
    () =>
      dispatch({
        type: "LAUNCH_SEARCH_RESULTS",
      }),
    [dispatch]
  );

  const handleCloseSearchResults = useCallback(
    () =>
      dispatch({
        type: "CLOSE_SEARCH_RESULTS",
      }),
    [dispatch]
  );

  const handleChangeResultsOrder = useCallback(
    (newResultsOrder: CombinedState["searchState"]["resultsOrder"]) =>
      dispatch({
        type: "CHANGE_RESULTS_ORDER",
        payload: newResultsOrder,
      }),
    [dispatch]
  );

  const handleUpdateFilter = useCallback(
    (payload: {
      filter: keyof CombinedState["searchState"]["filterOptions"];
      id: string;
      isSelected: boolean;
    }) => dispatch({ type: "UPDATE_FILTER", payload }),
    [dispatch]
  );

  const handleAddRedaction = useCallback(
    (pdfId: string, redaction: NewPdfHighlight) =>
      dispatch({
        type: "ADD_REDACTION_AND_ATTEMPT_LOCK",
        payload: { pdfId, redaction },
      }),
    [dispatch]
  );

  const handleRemoveRedaction = useCallback(
    (pdfId: string, redactionId: string) =>
      dispatch({
        type: "REMOVE_REDACTION_AND_POTENTIALLY_UNLOCK",
        payload: { pdfId, redactionId },
      }),
    [dispatch]
  );

  const handleRemoveAllRedactions = useCallback(
    (pdfId: string) =>
      dispatch({
        type: "REMOVE_ALL_REDACTIONS_AND_UNLOCK",
        payload: { pdfId },
      }),
    [dispatch]
  );

  return {
    ...combinedState,
    handleOpenPdf,
    handleClosePdf,
    handleSearchTermChange,
    handleLaunchSearchResults,
    handleCloseSearchResults,
    handleChangeResultsOrder,
    handleUpdateFilter,
    handleAddRedaction,
    handleRemoveRedaction,
    handleRemoveAllRedactions,
  };
};

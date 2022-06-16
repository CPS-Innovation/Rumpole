import { useCallback, useEffect, useReducer } from "react";
import { useApi } from "../../../../common/hooks/useApi";
import {
  getCaseDetails,
  getCaseDocumentsList,
  getHeaders,
} from "../../api/gateway-api";
import { usePipelineApi } from "../use-pipeline-api/usePipelineApi";
import { CombinedState } from "./CombinedState";
import { reducer } from "./reducer";

export const useCaseDetailsState = (id: string) => {
  const caseState = useApi(getCaseDetails, id);
  const documentsState = useApi(getCaseDocumentsList, id);
  const pipelineState = usePipelineApi(id);
  const headers = useApi(getHeaders);

  const [combinedState, dispatch] = useReducer(reducer, {
    caseState: { status: "loading" },
    documentsState: { status: "loading" },
    pipelineState: { status: "loading" },
    accordionState: { status: "loading" },
    tabsState: { items: [], authToken: undefined },
    searchState: {
      isResultsVisible: false,
      searchTerm: undefined,
    },
  } as CombinedState);

  useEffect(
    () => dispatch({ type: "UPDATE_CASE_DETAILS", payload: caseState }),
    [caseState]
  );

  useEffect(
    () => dispatch({ type: "UPDATE_CASE_DOCUMENTS", payload: documentsState }),
    [documentsState]
  );

  useEffect(
    () => dispatch({ type: "UPDATE_PIPELINE", payload: pipelineState }),
    [pipelineState]
  );

  useEffect(
    () => dispatch({ type: "UPDATE_AUTH_TOKEN", payload: headers }),
    [headers]
  );

  const handleOpenPdf = useCallback(
    (caseDocument: { tabSafeId: string; documentId: string }) => {
      dispatch({
        type: "OPEN_PDF",
        payload: {
          pdfId: caseDocument.documentId,
          tabSafeId: caseDocument.tabSafeId,
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

  const handleOpenSearchResults = useCallback(
    () =>
      dispatch({
        type: "OPEN_CLOSE_SEARCH_RESULTS",
        payload: { isOpen: true },
      }),
    [dispatch]
  );

  const handleCloseSearchResults = useCallback(
    () =>
      dispatch({
        type: "OPEN_CLOSE_SEARCH_RESULTS",
        payload: { isOpen: false },
      }),
    [dispatch]
  );

  return {
    caseState: combinedState.caseState,
    accordionState: combinedState.accordionState,
    tabsState: combinedState.tabsState,
    searchState: combinedState.searchState,
    handleOpenPdf,
    handleClosePdf,
    handleSearchTermChange,
    handleOpenSearchResults,
    handleCloseSearchResults,
  };
};

import { useCallback, useEffect, useReducer } from "react";
import { useApi } from "../../../../common/hooks/useApi";
import {
  getCaseDetails,
  getCaseDocumentsList,
  getHeaders,
} from "../../api/gateway-api";
import { usePipelineApi } from "../use-pipeline-api/usePipelineApi";
import { CaseDocumentWithTabSafeId } from "../../domain/CaseDocumentWithTabSafeId";
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
  });

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
    (caseDocument: CaseDocumentWithTabSafeId) => {
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

  return {
    caseState: combinedState.caseState,
    accordionState: combinedState.accordionState,
    tabsState: combinedState.tabsState,
    handleOpenPdf,
  };
};

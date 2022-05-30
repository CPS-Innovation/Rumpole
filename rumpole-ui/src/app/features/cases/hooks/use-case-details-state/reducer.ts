import { ApiResult } from "../../../../common/types/ApiResult";
import { resolvePdfUrl } from "../../api/gateway-api";
import { CaseDocumentWithUrl } from "../../domain/CaseDocumentWithUrl";
import { mapDocumentsToAccordionState } from "./documents-mapper";
import { CombinedState } from "./CombinedState";
import { CaseDetails } from "../../domain/CaseDetails";
import { CaseDocument } from "../../domain/CaseDocument";
import { PipelineResults } from "../../domain/PipelineResults";

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
        payload: ApiResult<PipelineResults>;
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

      const documentsState = action.payload;
      const accordionState =
        action.payload.status === "succeeded"
          ? mapDocumentsToAccordionState(documentsState)
          : state.accordionState;

      return { ...state, documentsState, accordionState };
    case "UPDATE_PIPELINE":
      if (action.payload.status === "failed") {
        throw action.payload.error;
      }

      const pipelineState = action.payload;
      if (pipelineState.status === "succeeded") {
        const openPdfsWeNeedToUpdate = pipelineState.data.documents
          .filter(
            (item) =>
              item.pdfBlobName &&
              state.tabsState.items.some(
                (tabItem) =>
                  tabItem.documentId === item.documentId && !tabItem.url
              )
          )
          .map(({ documentId, pdfBlobName }) => ({ documentId, pdfBlobName }));

        if (openPdfsWeNeedToUpdate.length) {
          const openTabs = state.tabsState.items.reduce((prev, curr) => {
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
            ...state,
            pipelineState,
            tabsState: { ...state.tabsState, items: openTabs },
          };
        }
      }

      return { ...state, pipelineState };
    case "OPEN_PDF":
      {
        const { pdfId, tabSafeId } = action.payload;
        if (
          !state.tabsState.items.some((item) => item.documentId === pdfId) &&
          state.documentsState.status === "succeeded"
        ) {
          const foundDocument = state.documentsState.data.find(
            (item) => item.documentId === pdfId
          )!;
          const blobName =
            state.pipelineState.status === "succeeded"
              ? state.pipelineState.data.documents.find(
                  (item) => item.documentId === pdfId
                )?.pdfBlobName
              : undefined;

          const url = blobName && resolvePdfUrl(blobName);

          return {
            ...state,
            tabsState: {
              ...state.tabsState,
              items: [
                ...state.tabsState.items,
                { ...foundDocument, url, tabSafeId },
              ],
            },
          };
        }
      }
      return state;
    case "CLOSE_PDF": {
      const { tabSafeId } = action.payload;
      console.log({ state, action });
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
    default:
      throw new Error("Unknown action passed to case details reducer");
  }
};

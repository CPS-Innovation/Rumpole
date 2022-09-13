import { Reducer } from "react";
import { AsyncActionHandlers } from "use-reducer-async";
import {
  checkinDocument,
  checkoutDocument,
  saveRedactions,
} from "../../api/gateway-api";
import { CaseDocumentViewModel } from "../../domain/CaseDocumentViewModel";
import { NewPdfHighlight } from "../../domain/NewPdfHighlight";
import { mapRedactionSaveRequest } from "./map-redaction-save-request";
import { reducer } from "./reducer";

const LOCKED_STATES_REQUIRING_UNLOCK: CaseDocumentViewModel["clientLockedState"][] =
  ["locked", "locking"];

const UNLOCKED_STATES_REQUIRING_LOCK: CaseDocumentViewModel["clientLockedState"][] =
  ["unlocked", "unlocking"];

type State = Parameters<typeof reducer>[0];
type Action = Parameters<typeof reducer>[1];

type AsyncActions =
  | {
      type: "ADD_REDACTION_AND_POTENTIALLY_LOCK";
      payload: { pdfId: string; redaction: NewPdfHighlight };
    }
  | {
      type: "REMOVE_REDACTION_AND_POTENTIALLY_UNLOCK";
      payload: {
        pdfId: string;
        redactionId: string;
      };
    }
  | {
      type: "REMOVE_ALL_REDACTIONS_AND_UNLOCK";
      payload: {
        pdfId: string;
      };
    }
  | {
      type: "SAVE_REDACTIONS";
      payload: {
        pdfId: string;
      };
    };

export const reducerAsyncActionHandlers: AsyncActionHandlers<
  Reducer<State, Action>,
  AsyncActions
> = {
  ADD_REDACTION_AND_POTENTIALLY_LOCK:
    ({ dispatch, getState }) =>
    async (action) => {
      const { payload } = action;

      const { pdfId } = payload;
      const {
        tabsState: { items },
        caseId,
      } = getState();

      const { clientLockedState } = items.find(
        (item) => item.documentId === pdfId
      )!;

      const documentRequiresLocking =
        UNLOCKED_STATES_REQUIRING_LOCK.includes(clientLockedState);

      dispatch({ type: "ADD_REDACTION", payload });

      if (!documentRequiresLocking) {
        return;
      }

      dispatch({
        type: "UPDATE_DOCUMENT_LOCK_STATE",
        payload: { pdfId, lockedState: "locking" },
      });

      const isLockSuccessful = await checkoutDocument(caseId, pdfId);

      dispatch({
        type: "UPDATE_DOCUMENT_LOCK_STATE",
        payload: {
          pdfId,
          lockedState: isLockSuccessful ? "locked" : "locked-by-other-user",
        },
      });
    },

  REMOVE_REDACTION_AND_POTENTIALLY_UNLOCK:
    ({ dispatch, getState }) =>
    async (action) => {
      const { payload } = action;

      const { pdfId } = payload;
      const {
        tabsState: { items },
        caseId,
      } = getState();

      const document = items.find((item) => item.documentId === pdfId)!;

      const { redactionHighlights, clientLockedState: lockedState } = document;

      dispatch({ type: "REMOVE_REDACTION", payload });

      const requiresCheckIn =
        // this is the last existing highlight
        redactionHighlights.length === 1 &&
        LOCKED_STATES_REQUIRING_UNLOCK.includes(lockedState);

      if (!requiresCheckIn) {
        return;
      }

      dispatch({
        type: "UPDATE_DOCUMENT_LOCK_STATE",
        payload: { pdfId, lockedState: "unlocking" },
      });

      await checkinDocument(caseId, pdfId);

      dispatch({
        type: "UPDATE_DOCUMENT_LOCK_STATE",
        payload: {
          pdfId,
          lockedState: "unlocked",
        },
      });
    },

  REMOVE_ALL_REDACTIONS_AND_UNLOCK:
    ({ dispatch, getState }) =>
    async (action) => {
      const { payload } = action;

      const { pdfId } = payload;
      const {
        tabsState: { items },
        caseId,
      } = getState();

      const document = items.find((item) => item.documentId === pdfId)!;

      const { clientLockedState: lockedState } = document;

      const requiresCheckIn =
        LOCKED_STATES_REQUIRING_UNLOCK.includes(lockedState);

      dispatch({ type: "REMOVE_ALL_REDACTIONS", payload });

      if (!requiresCheckIn) {
        return;
      }

      dispatch({
        type: "UPDATE_DOCUMENT_LOCK_STATE",
        payload: { pdfId, lockedState: "unlocking" },
      });

      await checkinDocument(caseId, pdfId);

      dispatch({
        type: "UPDATE_DOCUMENT_LOCK_STATE",
        payload: {
          pdfId,
          lockedState: "unlocked",
        },
      });
    },

  SAVE_REDACTIONS:
    ({ dispatch, getState }) =>
    async (action) => {
      const { payload } = action;
      const { pdfId } = payload;

      const {
        tabsState: { items },
        caseId,
      } = getState();

      const { redactionHighlights, pdfBlobName } = items.find(
        (item) => item.documentId === pdfId
      )!;

      const redactionSaveRequest = mapRedactionSaveRequest(
        pdfId,
        redactionHighlights
      );

      dispatch({
        type: "UPDATE_SAVED_STATE",
        payload: { savedState: "saving" },
      });

      const response = await saveRedactions(
        caseId,
        pdfId,
        pdfBlobName!, // todo: better typing, but we're guaranteed to have a pdfBlobName anyhow
        redactionSaveRequest
      );

      window.open(response.redactedDocumentUrl);

      // todo: does a save IN THE CGI API check a document in automatically?
      await checkinDocument(caseId, pdfId);

      dispatch({
        type: "UPDATE_SAVED_STATE",
        payload: { savedState: "saved" },
      });
    },
};

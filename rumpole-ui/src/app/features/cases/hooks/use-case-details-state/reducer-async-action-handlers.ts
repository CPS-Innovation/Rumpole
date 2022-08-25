import { Reducer } from "react";
import { AsyncActionHandlers } from "use-reducer-async";
import {
  checkinDocument,
  checkoutDocument,
  saveRedactions,
} from "../../api/gateway-api";
import { CaseDocumentViewModel } from "../../domain/CaseDocumentViewModel";
import { NewPdfHighlight } from "../../domain/NewPdfHighlight";
import { RedactionSaveRequest } from "../../domain/RedactionSaveRequest";
import { mapRedactionSaveRequest } from "./map-redaction-save-request";
import { reducer } from "./reducer";

const LOCK_STATES_REQUIRING_UNLOCK: CaseDocumentViewModel["lockedState"][] = [
  "locked",
  "locking",
];

type State = Parameters<typeof reducer>[0];
type Action = Parameters<typeof reducer>[1];

type AsyncActions =
  | {
      type: "ADD_REDACTION_AND_ATTEMPT_LOCK";
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
  ADD_REDACTION_AND_ATTEMPT_LOCK:
    ({ dispatch, getState }) =>
    async (action) => {
      const { payload } = action;

      const { pdfId } = payload;
      const {
        tabsState: { items },
        caseId,
      } = getState();

      const documentRequiresLocking =
        items.find((item) => item.documentId === pdfId)?.lockedState ===
        "unlocked";

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

      const { redactionHighlights, lockedState } = document;

      const requiresCheckIn =
        // this is the last existing highlight
        redactionHighlights.length === 1 &&
        LOCK_STATES_REQUIRING_UNLOCK.includes(lockedState);

      dispatch({ type: "REMOVE_REDACTION", payload });

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

      const { lockedState } = document;

      const requiresCheckIn =
        LOCK_STATES_REQUIRING_UNLOCK.includes(lockedState);

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

      const { redactionHighlights } = items.find(
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
        redactionSaveRequest
      );

      console.log(response);

      dispatch({
        type: "UPDATE_SAVED_STATE",
        payload: { savedState: "saved" },
      });
    },
};

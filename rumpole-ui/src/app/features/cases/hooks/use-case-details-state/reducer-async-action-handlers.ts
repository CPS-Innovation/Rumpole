import { Reducer } from "react";
import { AsyncActionHandlers } from "use-reducer-async";
import { checkoutDocument } from "../../api/gateway-api";
import { NewPdfHighlight } from "../../domain/NewPdfHighlight";
import { reducer } from "./reducer";

type State = Parameters<typeof reducer>[0];
type Action = Parameters<typeof reducer>[1];

type AsyncAction = {
  type: "ADD_REDACTION_AND_ATTEMPT_LOCK";
  payload: { pdfId: string; redaction: NewPdfHighlight };
};

export const reducerAsyncActionHandlers: AsyncActionHandlers<
  Reducer<State, Action>,
  AsyncAction
> = {
  ADD_REDACTION_AND_ATTEMPT_LOCK:
    ({ dispatch, getState }) =>
    async (action) => {
      const { payload } = action;

      dispatch({ type: "ADD_REDACTION", payload });

      const { pdfId } = payload;
      const { tabsState, caseId } = getState();

      const documentRequiresLocking =
        tabsState.items.find((item) => item.documentId === pdfId)
          ?.lockedState === "not-yet-locked";

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
};

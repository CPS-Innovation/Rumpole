import { act } from "react-dom/test-utils";
import { CaseDocumentViewModel } from "../../domain/CaseDocumentViewModel";
import { CombinedState } from "../../domain/CombinedState";
import { NewPdfHighlight } from "../../domain/NewPdfHighlight";
import { reducerAsyncActionHandlers } from "./reducer-async-action-handlers";
import * as api from "../../api/gateway-api";

describe("reducerAsyncActionHandlers", () => {
  it("can add a redaction and lock the document", () => {
    const mockGetCaseDetails = jest
      .spyOn(api, "checkoutDocument")
      .mockImplementation(() => Promise.resolve(true));

    const dispatchMock = jest.fn();

    const combinedState = {
      tabsState: { items: [] as CaseDocumentViewModel[] },
      caseId: "bar",
    } as CombinedState;

    const { ADD_REDACTION_AND_POTENTIALLY_LOCK } = reducerAsyncActionHandlers;

    const handler = ADD_REDACTION_AND_POTENTIALLY_LOCK({
      dispatch: dispatchMock,
      getState: () => combinedState,
      signal: new AbortController().signal,
    });

    handler({
      type: "ADD_REDACTION_AND_POTENTIALLY_LOCK",
      payload: { pdfId: "foo", redaction: {} as NewPdfHighlight },
    });
  });
});

export {};

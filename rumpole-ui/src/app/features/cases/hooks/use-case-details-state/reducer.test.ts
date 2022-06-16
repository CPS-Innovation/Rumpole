import { CombinedState } from "./CombinedState";
import { reducer } from "./reducer";
import * as documentsMapper from "./documents-mapper";
import * as apiGateway from "../../api/gateway-api";
import { ApiResult } from "../../../../common/types/ApiResult";

const ERROR = new Error();

describe("useCaseDetailsState reducer", () => {
  it("throws if update caseState fails", () => {
    expect(() =>
      reducer({} as CombinedState, {
        type: "UPDATE_CASE_DETAILS",
        payload: { status: "failed", error: ERROR, httpStatusCode: undefined },
      })
    ).toThrowError(ERROR);
  });

  it("can update case details", () => {
    const newCaseState = {} as CombinedState["caseState"];

    const nextState = reducer({} as CombinedState, {
      type: "UPDATE_CASE_DETAILS",
      payload: newCaseState,
    });

    expect(nextState.caseState).toBe(newCaseState);
  });

  it("throws if update documentsState fails", () => {
    expect(() =>
      reducer({} as CombinedState, {
        type: "UPDATE_CASE_DOCUMENTS",
        payload: {
          status: "failed",
          error: ERROR,
          httpStatusCode: undefined,
        },
      })
    ).toThrowError(ERROR);
  });

  it("can not update accordionState if incoming documentsState is not ready", () => {
    const newDocumentsState = {
      status: "loading",
    } as CombinedState["documentsState"];

    const exitingAccordionState = {} as CombinedState["accordionState"];

    const nextState = reducer(
      { accordionState: exitingAccordionState } as CombinedState,
      {
        type: "UPDATE_CASE_DOCUMENTS",
        payload: newDocumentsState,
      }
    );

    expect(nextState.documentsState).toBe(newDocumentsState);
    expect(nextState.accordionState).toBe(exitingAccordionState);
  });

  it("can update accordionState if incoming documentsState is ready", () => {
    const existingAccordionState = {} as CombinedState["accordionState"];
    const newAccordionState = {} as CombinedState["accordionState"];
    const newDocumentsState = {
      status: "succeeded",
    } as CombinedState["documentsState"];

    jest
      .spyOn(documentsMapper, "mapDocumentsToAccordionState")
      .mockImplementation((documentsState) => {
        if (documentsState !== newDocumentsState) throw new Error();
        return newAccordionState;
      });

    const nextState = reducer(
      { accordionState: existingAccordionState } as CombinedState,
      {
        type: "UPDATE_CASE_DOCUMENTS",
        payload: newDocumentsState,
      }
    );

    expect(nextState.documentsState).toBe(newDocumentsState);
    expect(nextState.accordionState).toBe(newAccordionState);
  });

  it("throws if update pipelineState fails", () => {
    expect(() =>
      reducer({} as CombinedState, {
        type: "UPDATE_PIPELINE",
        payload: {
          status: "failed",
          error: ERROR,
          httpStatusCode: undefined,
        },
      })
    ).toThrowError(ERROR);
  });

  it("can update from pipeline", () => {
    const newPipelineState = {} as CombinedState["pipelineState"];

    const nextState = reducer({} as CombinedState, {
      type: "UPDATE_PIPELINE",
      payload: newPipelineState,
    });

    expect(nextState.pipelineState).toBe(newPipelineState);
  });

  it("can update from pipeline when no tabs are open", () => {
    const newPipelineState = {
      status: "succeeded",
      data: {
        documents: [
          {
            documentId: "d2",
            pdfBlobName: "foo",
          },
        ],
      },
    } as CombinedState["pipelineState"];

    const existingTabsState = {
      items: [],
      authToken: undefined,
    } as CombinedState["tabsState"];

    const nextState = reducer(
      { tabsState: existingTabsState } as CombinedState,
      {
        type: "UPDATE_PIPELINE",
        payload: newPipelineState,
      }
    );

    expect(nextState.tabsState).toBe(existingTabsState);
  });

  it("can update from pipeline tabs already open with pdf url", () => {
    const newPipelineState = {
      status: "succeeded",
      data: {
        documents: [
          {
            documentId: "d2",
            pdfBlobName: "foo",
          },
        ],
      },
    } as CombinedState["pipelineState"];

    const existingTabsState = {
      items: [
        { documentId: "d1", url: undefined },
        { documentId: "d2", url: undefined },
        { documentId: "d3", url: undefined },
      ],
    } as CombinedState["tabsState"];

    jest.spyOn(apiGateway, "resolvePdfUrl").mockImplementation((blobName) => {
      if (blobName !== "foo") throw new Error();
      return "baz";
    });

    const nextState = reducer(
      { tabsState: existingTabsState } as CombinedState,
      {
        type: "UPDATE_PIPELINE",
        payload: newPipelineState,
      }
    );

    expect(nextState.tabsState).toEqual({
      items: [
        { documentId: "d1", url: undefined },
        { documentId: "d2", url: "baz" },
        { documentId: "d3", url: undefined },
      ],
    });
  });

  it("can open a tab when the pdf pdf details are known", () => {
    const existingTabsState = {
      authToken: "authtoken",
      items: [],
    } as CombinedState["tabsState"];
    const existingDocumentsState = {
      status: "succeeded",
      data: [{ documentId: "d1" }],
    } as CombinedState["documentsState"];
    const existingPipelineState = {
      status: "succeeded",
      data: {
        transationId: "",
        documents: [{ documentId: "d1", pdfBlobName: "foo" }],
      },
    } as CombinedState["pipelineState"];

    jest.spyOn(apiGateway, "resolvePdfUrl").mockImplementation((blobName) => {
      if (blobName !== "foo") throw new Error();
      return "baz";
    });

    const nextState = reducer(
      {
        documentsState: existingDocumentsState,
        pipelineState: existingPipelineState,
        tabsState: existingTabsState,
      } as CombinedState,
      { type: "OPEN_PDF", payload: { pdfId: "d1", tabSafeId: "t1" } }
    );

    expect(nextState.tabsState).toEqual({
      authToken: "authtoken",
      items: [
        {
          documentId: "d1",
          url: "baz",
          tabSafeId: "t1",
        },
      ],
    });
  });

  it("can open a tab when the pdf pdf details are not known", () => {
    const existingTabsState = {
      authToken: "authtoken",
      items: [],
    } as CombinedState["tabsState"];
    const existingDocumentsState = {
      status: "succeeded",
      data: [{ documentId: "d1" }],
    } as CombinedState["documentsState"];
    const existingPipelineState = {
      status: "loading",
    } as CombinedState["pipelineState"];

    const nextState = reducer(
      {
        documentsState: existingDocumentsState,
        pipelineState: existingPipelineState,
        tabsState: existingTabsState,
      } as CombinedState,
      { type: "OPEN_PDF", payload: { pdfId: "d1", tabSafeId: "t1" } }
    );

    expect(nextState.tabsState).toEqual({
      authToken: "authtoken",
      items: [
        {
          documentId: "d1",
          url: undefined,
          tabSafeId: "t1",
        },
      ],
    });
  });

  it("can reopen a tab that is already open", () => {
    const existingTabsState = {
      authToken: "authtoken",
      items: [
        {
          documentId: "d1",
          url: "baz",
          tabSafeId: "t1",
        },
      ],
    } as CombinedState["tabsState"];

    const nextState = reducer(
      {
        tabsState: existingTabsState,
      } as CombinedState,
      { type: "OPEN_PDF", payload: { pdfId: "d1", tabSafeId: "t1" } }
    );

    expect(nextState.tabsState).toBe(existingTabsState);
  });

  it("can close a tab", () => {
    const existingTabsState = {
      authToken: "authtoken",
      items: [
        {
          documentId: "d1",
          url: undefined,
          tabSafeId: "t1",
        },
        {
          documentId: "d2",
          url: undefined,
          tabSafeId: "t2",
        },
      ],
    } as CombinedState["tabsState"];

    const nextState = reducer(
      {
        tabsState: existingTabsState,
      } as CombinedState,
      { type: "CLOSE_PDF", payload: { tabSafeId: "t2" } }
    );

    expect(nextState.tabsState).toEqual({
      authToken: "authtoken",
      items: [
        {
          documentId: "d1",
          url: undefined,
          tabSafeId: "t1",
        },
      ],
    });
  });

  it("throws if update auth token fails", () => {
    expect(() =>
      reducer({} as CombinedState, {
        type: "UPDATE_AUTH_TOKEN",
        payload: {
          status: "failed",
          error: ERROR,
          httpStatusCode: undefined,
        },
      })
    ).toThrowError(ERROR);
  });

  it("can not update auth token if headers are not ready", () => {
    const existingState = {} as CombinedState;
    const newHeaders = { status: "loading" } as ApiResult<Headers>;

    const newState = reducer(existingState, {
      type: "UPDATE_AUTH_TOKEN",
      payload: newHeaders,
    });
    expect(newState).toBe(existingState);
  });

  it("can not update auth token if headers are not present", () => {
    const existingTabsState = {
      items: [],
      authToken: undefined,
    } as CombinedState["tabsState"];

    const newHeadersApiResult = {
      status: "succeeded",
      data: new Headers(),
    } as ApiResult<Headers>;

    const newState = reducer(
      { tabsState: existingTabsState } as CombinedState,
      {
        type: "UPDATE_AUTH_TOKEN",
        payload: newHeadersApiResult,
      }
    );
    expect(newState.tabsState).toEqual({ items: [], authToken: undefined });
  });

  it("can update auth token if headers are present", () => {
    const existingTabsState = {
      items: [],
      authToken: undefined,
    } as CombinedState["tabsState"];

    const headers = new Headers();
    headers.append("Authorization", "auth-token");

    const newHeadersApiResult = {
      status: "succeeded",
      data: headers,
    } as ApiResult<Headers>;

    const newState = reducer(
      { tabsState: existingTabsState } as CombinedState,
      {
        type: "UPDATE_AUTH_TOKEN",
        payload: newHeadersApiResult,
      }
    );
    expect(newState.tabsState).toEqual({ items: [], authToken: "auth-token" });
  });

  it("can update search term", () => {
    const existingSearchState = {
      searchTerm: "foo",
      isResultsVisible: false,
    } as CombinedState["searchState"];

    const newState = reducer(
      { searchState: existingSearchState } as CombinedState,
      { type: "UPDATE_SEARCH_TERM", payload: { searchTerm: "bar" } }
    );

    expect(newState.searchState).toEqual({
      searchTerm: "bar",
      isResultsVisible: false,
    });
  });

  it("can open search results", () => {
    const existingSearchState = {
      searchTerm: "foo",
      isResultsVisible: false,
    } as CombinedState["searchState"];

    const newState = reducer(
      { searchState: existingSearchState } as CombinedState,
      { type: "OPEN_CLOSE_SEARCH_RESULTS", payload: { isOpen: true } }
    );

    expect(newState.searchState).toEqual({
      searchTerm: "foo",
      isResultsVisible: true,
    });
  });

  it("can close search results", () => {
    const existingSearchState = {
      searchTerm: "foo",
      isResultsVisible: true,
    } as CombinedState["searchState"];

    const newState = reducer(
      { searchState: existingSearchState } as CombinedState,
      { type: "OPEN_CLOSE_SEARCH_RESULTS", payload: { isOpen: false } }
    );

    expect(newState.searchState).toEqual({
      searchTerm: "foo",
      isResultsVisible: false,
    });
  });

  it("can not handle an unknown action", () => {
    expect(() =>
      reducer({} as CombinedState, { type: "UNKNOWN" } as any)
    ).toThrow();
  });
});

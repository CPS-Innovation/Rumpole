import { useCaseDetailsState, initialState } from "./useCaseDetailsState";
import * as searchCaseWhenReady from "./search-case-when-ready";
import * as api from "../../api/gateway-api";
import * as pipelineApi from "../use-pipeline-api/usePipelineApi";
import { CaseSearchResult } from "../../domain/CaseSearchResult";
import { CaseDocument } from "../../domain/CaseDocument";
import { ApiTextSearchResult } from "../../domain/ApiTextSearchResult";
import { PipelineResults } from "../../domain/PipelineResults";
import { AsyncPipelineResult } from "../use-pipeline-api/AsyncPipelineResult";
import { renderHook } from "@testing-library/react-hooks";
import * as useApi from "../../../../common/hooks/useApi";
import * as reducer from "./reducer";
import { act } from "react-dom/test-utils";
import { NewPdfHighlight } from "../../domain/NewPdfHighlight";
import { reducerAsyncActionHandlers } from "./reducer-async-action-handlers";

type ReducerParams = Parameters<typeof reducer.reducer>;
let reducerSpy: jest.SpyInstance<ReducerParams[0]>;

const isSameRef = (a: any, b: any) => a === b;

describe("useCaseDetailsState", () => {
  beforeEach(() => {
    const mockGetCaseDetails = jest
      .spyOn(api, "getCaseDetails")
      .mockImplementation(
        () =>
          new Promise((resolve) =>
            setTimeout(() => resolve({} as CaseSearchResult), 100)
          )
      );

    const mockgetCaseDocumentsList = jest
      .spyOn(api, "getCaseDocumentsList")
      .mockImplementation(
        () =>
          new Promise((resolve) =>
            setTimeout(() => resolve([] as CaseDocument[]), 100)
          )
      );

    const mockGetHeaders = jest
      .spyOn(api, "getCoreHeaders")
      .mockImplementation(
        () =>
          new Promise((resolve) =>
            setTimeout(() => resolve(new Headers()), 100)
          )
      );

    const mockSearchCaseWhenAllReady = jest
      .spyOn(searchCaseWhenReady, "searchCaseWhenReady")
      .mockImplementation(
        () =>
          new Promise((resolve) =>
            setTimeout(() => resolve([] as ApiTextSearchResult[]), 100)
          )
      );

    const mockSearchCase = jest
      .spyOn(api, "searchCase")
      .mockImplementation(
        () =>
          new Promise((resolve) =>
            setTimeout(() => resolve([] as ApiTextSearchResult[]), 100)
          )
      );

    jest.spyOn(useApi, "useApi").mockImplementation((del, p0, p1, p2, p3) => {
      if (isSameRef(del, mockGetCaseDetails)) {
        return { status: "succeeded", data: "getCaseDetails" };
      }
      if (isSameRef(del, mockgetCaseDocumentsList)) {
        return { status: "succeeded", data: "getCaseDocumentsList" };
      }
      if (isSameRef(del, mockGetHeaders)) {
        return { status: "succeeded", data: "getHeaders" };
      }
      if (isSameRef(del, mockSearchCase)) {
        return {
          status: "succeeded",
          data: "searchCase",
        };
      }
      if (isSameRef(del, mockSearchCaseWhenAllReady)) {
        return {
          status: "succeeded",
          data: "searchCaseWhenAllReady",
        };
      }
      throw new Error("Should not be here");
    });

    jest
      .spyOn(pipelineApi, "usePipelineApi")
      .mockImplementation(() => ({} as AsyncPipelineResult<PipelineResults>));

    reducerSpy = jest
      .spyOn(reducer, "reducer")
      .mockImplementation((state) => state);
  });

  afterEach(() => jest.restoreAllMocks());

  describe("initialisation", () => {
    it("initialises to the expected state", () => {
      const { result } = renderHook(() => useCaseDetailsState("foo"));

      const {
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
        handleSavedRedactions,
        ...stateProperties
      } = result.current;

      expect(stateProperties).toEqual({ caseId: "foo", ...initialState });
    });

    it("can update state according to the api call results", async () => {
      renderHook(() => useCaseDetailsState("foo"));

      expect(reducerSpy).toBeCalledWith(expect.anything(), {
        type: "UPDATE_CASE_DETAILS",
        payload: { status: "succeeded", data: "getCaseDetails" },
      });

      expect(reducerSpy).toBeCalledWith(expect.anything(), {
        type: "UPDATE_CASE_DOCUMENTS",
        payload: { status: "succeeded", data: "getCaseDocumentsList" },
      });

      expect(reducerSpy).toBeCalledWith(expect.anything(), {
        type: "UPDATE_PIPELINE",
        payload: {},
      });

      expect(reducerSpy).toBeCalledWith(expect.anything(), {
        type: "UPDATE_SEARCH_RESULTS",
        payload: {
          status: "succeeded",
          data: "searchCaseWhenAllReady",
        },
      });
    });
  });

  describe("synchronous action handlers", () => {
    it("can close a pdf", () => {
      const {
        result: {
          current: { handleClosePdf },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      act(() => handleClosePdf({ tabSafeId: "1" }));

      expect(reducerSpy).toBeCalledWith(expect.anything(), {
        type: "CLOSE_PDF",
        payload: { tabSafeId: "1" },
      });
    });

    it("can update search term", () => {
      const {
        result: {
          current: { handleSearchTermChange },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      act(() => handleSearchTermChange("foo"));

      expect(reducerSpy).toBeCalledWith(expect.anything(), {
        type: "UPDATE_SEARCH_TERM",
        payload: {
          searchTerm: "foo",
        },
      });
    });

    it("can launch search results", () => {
      const {
        result: {
          current: { handleLaunchSearchResults },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      act(() => handleLaunchSearchResults());

      expect(reducerSpy).toBeCalledWith(expect.anything(), {
        type: "LAUNCH_SEARCH_RESULTS",
      });
    });

    it("can close search results", () => {
      const {
        result: {
          current: { handleCloseSearchResults },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      act(() => handleCloseSearchResults());

      expect(reducerSpy).toBeCalledWith(expect.anything(), {
        type: "CLOSE_SEARCH_RESULTS",
      });
    });

    it("can change results order", () => {
      const {
        result: {
          current: { handleChangeResultsOrder },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      act(() => handleChangeResultsOrder("byDateDesc"));

      expect(reducerSpy).toBeCalledWith(expect.anything(), {
        type: "CHANGE_RESULTS_ORDER",
        payload: "byDateDesc",
      });
    });

    it("can update a filter", () => {
      const {
        result: {
          current: { handleUpdateFilter },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      act(() =>
        handleUpdateFilter({ filter: "category", id: "1", isSelected: true })
      );

      expect(reducerSpy).toBeCalledWith(expect.anything(), {
        type: "UPDATE_FILTER",
        payload: { filter: "category", id: "1", isSelected: true },
      });
    });
  });
  describe("async action handlers", () => {
    it("can open a pdf", () => {
      const mockHandler = jest.fn();

      jest
        .spyOn(reducerAsyncActionHandlers, "REQUEST_OPEN_PDF")
        .mockImplementation(() => mockHandler);

      const {
        result: {
          current: { handleOpenPdf },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      act(() =>
        handleOpenPdf({ tabSafeId: "1", documentId: "2", mode: "read" })
      );

      expect(mockHandler).toBeCalledWith({
        type: "REQUEST_OPEN_PDF",
        payload: { tabSafeId: "1", pdfId: "2", mode: "read" },
      });
    });

    it("can add a redaction", () => {
      const mockHandler = jest.fn();

      jest
        .spyOn(reducerAsyncActionHandlers, "ADD_REDACTION_AND_POTENTIALLY_LOCK")
        .mockImplementation(() => mockHandler);

      const {
        result: {
          current: { handleAddRedaction },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      handleAddRedaction("bar", { type: "redaction" } as NewPdfHighlight);

      expect(mockHandler).toBeCalledWith({
        type: "ADD_REDACTION_AND_POTENTIALLY_LOCK",
        payload: { pdfId: "bar", redaction: { type: "redaction" } },
      });
    });

    it("can remove a redaction", () => {
      const mockHandler = jest.fn();

      jest
        .spyOn(
          reducerAsyncActionHandlers,
          "REMOVE_REDACTION_AND_POTENTIALLY_UNLOCK"
        )
        .mockImplementation(() => mockHandler);

      const {
        result: {
          current: { handleRemoveRedaction },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      handleRemoveRedaction("bar", "baz");

      expect(mockHandler).toBeCalledWith({
        type: "REMOVE_REDACTION_AND_POTENTIALLY_UNLOCK",
        payload: { pdfId: "bar", redactionId: "baz" },
      });
    });

    it("can remove all redactions", () => {
      const mockHandler = jest.fn();

      jest
        .spyOn(reducerAsyncActionHandlers, "REMOVE_ALL_REDACTIONS_AND_UNLOCK")
        .mockImplementation(() => mockHandler);

      const {
        result: {
          current: { handleRemoveAllRedactions },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      handleRemoveAllRedactions("bar");

      expect(mockHandler).toBeCalledWith({
        type: "REMOVE_ALL_REDACTIONS_AND_UNLOCK",
        payload: { pdfId: "bar" },
      });
    });

    it("can save all redactions", () => {
      const mockHandler = jest.fn();

      jest
        .spyOn(reducerAsyncActionHandlers, "SAVE_REDACTIONS")
        .mockImplementation(() => mockHandler);

      const {
        result: {
          current: { handleSavedRedactions },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      handleSavedRedactions("bar");

      expect(mockHandler).toBeCalledWith({
        type: "SAVE_REDACTIONS",
        payload: { pdfId: "bar" },
      });
    });
  });
});

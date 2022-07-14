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
      .spyOn(api, "getHeaders")
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

  afterEach(jest.restoreAllMocks);

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
        ...stateProperties
      } = result.current;

      expect(stateProperties).toEqual(initialState);
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
        type: "UPDATE_AUTH_TOKEN",
        payload: { status: "succeeded", data: "getHeaders" },
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

  describe("handlers", () => {
    it("can open a pdf", () => {
      const {
        result: {
          current: { handleOpenPdf },
        },
      } = renderHook(() => useCaseDetailsState("foo"));

      act(() => handleOpenPdf({ tabSafeId: "1", documentId: "2" }));

      expect(reducerSpy).toBeCalledWith(expect.anything(), {
        type: "OPEN_PDF",
        payload: { tabSafeId: "1", pdfId: "2" },
      });
    });

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
});

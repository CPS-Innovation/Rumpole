import { renderHook, act } from "@testing-library/react-hooks";
import { CaseSearchResult } from "../domain/CaseSearchResult";

import { CaseFilterQueryParams } from "./types/CaseFilterQueryParams";
import { QueryParamsState } from "./useQueryParams";
import { SearchDataState } from "./useSearchDataState";
import { useSearchState } from "./useSearchState";

const mockSetParams = jest.fn();

let mockFilterState: QueryParamsState<CaseFilterQueryParams>;

let mockDataState: SearchDataState;

beforeEach(() => {
  mockSetParams.mockReset();

  mockFilterState = {
    params: {
      urn: "1",
    },
    setParams: mockSetParams,
  };

  mockDataState = {
    data: [],
    reduxUrn: "2",
    loadingStatus: "idle",
  };
});

describe("useSearchState", () => {
  test("returns urn from query params", () => {
    mockFilterState.params.urn = "foo";
    mockDataState.reduxUrn = "bar";
    const {
      result: {
        current: { urn },
      },
    } = renderHook(() => useSearchState(mockFilterState, mockDataState));

    expect(urn).toEqual("foo");
  });

  test("returns expected loadingStatus when query urn equals data urn", () => {
    mockDataState.reduxUrn = "foo";
    mockFilterState.params.urn = "foo";
    mockDataState.loadingStatus = "loading";
    const {
      result: {
        current: { loadingStatus },
      },
    } = renderHook(() => useSearchState(mockFilterState, mockDataState));

    expect(loadingStatus).toBe("loading");
  });

  test("returns idle when query urn does not equal data urn", () => {
    mockDataState.reduxUrn = "foo";
    mockFilterState.params.urn = "bar";
    mockDataState.loadingStatus = "loading";
    const {
      result: {
        current: { loadingStatus },
      },
    } = renderHook(() => useSearchState(mockFilterState, mockDataState));

    expect(loadingStatus).toBe("idle");
  });

  describe("filtering", () => {
    test("it can filter on a single area", () => {
      mockDataState.data = [
        {
          area: {
            code: "1",
            name: "foo",
          },
          agency: {},
          status: {},
        },
        { area: { code: "2", name: "bar" }, agency: {}, status: {} },
        { area: { code: "1", name: "foo" }, agency: {}, status: {} },
        { area: { code: "3", name: "baz" }, agency: {}, status: {} },
      ] as CaseSearchResult[];

      mockFilterState.params.area = ["1"];
      const {
        result: {
          current: { filters, isFiltered, filteredData },
        },
      } = renderHook(() => useSearchState(mockFilterState, mockDataState));

      // expect(filters.area).toEqual({
      //   isActive: true,
      //   name: "area"
      // });
    });
  });

  test("returns totalCount", () => {
    mockDataState.data = [
      { area: {}, agency: {}, status: {} },
      { area: {}, agency: {}, status: {} },
      { area: {}, agency: {}, status: {} },
    ] as CaseSearchResult[];
    const {
      result: {
        current: { totalCount },
      },
    } = renderHook(() => useSearchState(mockFilterState, mockDataState));

    expect(totalCount).toBe(3);
  });

  test("set query param urn", () => {
    const {
      result: {
        current: { setUrnParam },
      },
    } = renderHook(() => useSearchState(mockFilterState, mockDataState));

    act(() => setUrnParam("buzz"));

    expect(mockSetParams).toHaveBeenCalledTimes(1);
    expect(mockSetParams).toHaveBeenCalledWith({ urn: "buzz" });
  });
});

/*
  returns urn
  sets urn

  loadingStatus

  totalCount

*/

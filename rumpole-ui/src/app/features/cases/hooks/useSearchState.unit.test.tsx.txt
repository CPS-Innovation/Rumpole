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

  test("returns totalCount", () => {
    mockDataState.data = [
      { area: {}, agency: {} },
      { area: {}, agency: {} },
      { area: {}, agency: {} },
    ] as CaseSearchResult[];

    const {
      result: {
        current: { totalCount },
      },
    } = renderHook(() => useSearchState(mockFilterState, mockDataState));

    expect(totalCount).toBe(3);
  });

  describe("filtering", () => {
    describe("area", () => {
      beforeEach(() => {
        mockDataState.data = [
          {
            area: {
              code: "1",
              name: "foo",
            },
            agency: {},
          },
          { area: { code: "2", name: "bar" }, agency: {} },
          { area: { code: "1", name: "foo" }, agency: {} },
          { area: { code: "3", name: "baz" }, agency: {} },
        ] as CaseSearchResult[];
      });

      test("it can filter on a single area", () => {
        mockFilterState.params.area = ["1"];

        const {
          result: {
            current: { filters, isFiltered, filteredData },
          },
        } = renderHook(() => useSearchState(mockFilterState, mockDataState));

        expect(filters.area).toEqual({
          isActive: true,
          name: "area",
          items: [
            { count: 1, id: "2", isSelected: false, name: "bar" },
            { count: 1, id: "3", isSelected: false, name: "baz" },
            { count: 2, id: "1", isSelected: true, name: "foo" },
          ],
        });
        expect(isFiltered).toBe(true);
        expect(filteredData).toEqual([
          {
            area: {
              code: "1",
              name: "foo",
            },
            agency: {},
          },
          { area: { code: "1", name: "foo" }, agency: {} },
        ]);
      });

      test("it can filter on multiple areas", () => {
        mockFilterState.params.area = ["1", "3"];

        const {
          result: {
            current: { filters, isFiltered, filteredData },
          },
        } = renderHook(() => useSearchState(mockFilterState, mockDataState));

        expect(filters.area).toEqual({
          isActive: true,
          name: "area",
          items: [
            { count: 1, id: "2", isSelected: false, name: "bar" },
            { count: 1, id: "3", isSelected: true, name: "baz" },
            { count: 2, id: "1", isSelected: true, name: "foo" },
          ],
        });
        expect(isFiltered).toBe(true);
        expect(filteredData).toEqual([
          {
            area: {
              code: "1",
              name: "foo",
            },
            agency: {},
          },
          { area: { code: "1", name: "foo" }, agency: {} },
          { area: { code: "3", name: "baz" }, agency: {} },
        ]);
      });

      test("it can filter on no areas", () => {
        mockFilterState.params.area = [];

        const {
          result: {
            current: { filters, isFiltered, filteredData },
          },
        } = renderHook(() => useSearchState(mockFilterState, mockDataState));

        expect(filters.area).toEqual({
          isActive: true,
          name: "area",
          items: [
            { count: 1, id: "2", isSelected: false, name: "bar" },
            { count: 1, id: "3", isSelected: false, name: "baz" },
            { count: 2, id: "1", isSelected: false, name: "foo" },
          ],
        });
        expect(isFiltered).toBe(true);
        expect(filteredData).toEqual([]);
      });

      test("it can not filter on area and show all data", () => {
        mockFilterState.params.area = undefined;

        const {
          result: {
            current: { filters, isFiltered, filteredData },
          },
        } = renderHook(() => useSearchState(mockFilterState, mockDataState));

        expect(filters.area).toEqual({
          isActive: true,
          name: "area",
          items: [
            { count: 1, id: "2", isSelected: true, name: "bar" },
            { count: 1, id: "3", isSelected: true, name: "baz" },
            { count: 2, id: "1", isSelected: true, name: "foo" },
          ],
        });
        expect(isFiltered).toBe(true);
        expect(filteredData).toEqual(mockDataState.data);
      });
    });
    describe("agency", () => {
      beforeEach(() => {
        mockDataState.data = [
          {
            agency: {
              code: "1",
              name: "foo",
            },
            area: {},
          },
          { agency: { code: "2", name: "bar" }, area: {} },
          { agency: { code: "1", name: "foo" }, area: {} },
          { agency: { code: "3", name: "baz" }, area: {} },
        ] as CaseSearchResult[];
      });

      test("it can filter on a single agency", () => {
        mockFilterState.params.agency = ["1"];

        const {
          result: {
            current: { filters, isFiltered, filteredData },
          },
        } = renderHook(() => useSearchState(mockFilterState, mockDataState));

        expect(filters.agency).toEqual({
          isActive: true,
          name: "agency",
          items: [
            { count: 1, id: "2", isSelected: false, name: "bar" },
            { count: 1, id: "3", isSelected: false, name: "baz" },
            { count: 2, id: "1", isSelected: true, name: "foo" },
          ],
        });
        expect(isFiltered).toBe(true);
        expect(filteredData).toEqual([
          {
            agency: {
              code: "1",
              name: "foo",
            },
            area: {},
          },
          { agency: { code: "1", name: "foo" }, area: {} },
        ]);
      });

      test("it can filter on multiple agencies", () => {
        mockFilterState.params.agency = ["1", "3"];

        const {
          result: {
            current: { filters, isFiltered, filteredData },
          },
        } = renderHook(() => useSearchState(mockFilterState, mockDataState));

        expect(filters.agency).toEqual({
          isActive: true,
          name: "agency",
          items: [
            { count: 1, id: "2", isSelected: false, name: "bar" },
            { count: 1, id: "3", isSelected: true, name: "baz" },
            { count: 2, id: "1", isSelected: true, name: "foo" },
          ],
        });
        expect(isFiltered).toBe(true);
        expect(filteredData).toEqual([
          {
            agency: {
              code: "1",
              name: "foo",
            },
            area: {},
          },
          { agency: { code: "1", name: "foo" }, area: {} },
          { agency: { code: "3", name: "baz" }, area: {} },
        ]);
      });

      test("it can filter on no agencies and show all data", () => {
        mockFilterState.params.agency = [];

        const {
          result: {
            current: { filters, isFiltered, filteredData },
          },
        } = renderHook(() => useSearchState(mockFilterState, mockDataState));

        expect(filters.agency).toEqual({
          isActive: true,
          name: "agency",
          items: [
            { count: 1, id: "2", isSelected: false, name: "bar" },
            { count: 1, id: "3", isSelected: false, name: "baz" },
            { count: 2, id: "1", isSelected: false, name: "foo" },
          ],
        });
        expect(isFiltered).toBe(true);
        expect(filteredData).toEqual([]);
      });

      test("it can not filter on agency", () => {
        mockFilterState.params.agency = undefined;

        const {
          result: {
            current: { filters, isFiltered, filteredData },
          },
        } = renderHook(() => useSearchState(mockFilterState, mockDataState));

        expect(filters.agency).toEqual({
          isActive: true,
          name: "agency",
          items: [
            { count: 1, id: "2", isSelected: true, name: "bar" },
            { count: 1, id: "3", isSelected: true, name: "baz" },
            { count: 2, id: "1", isSelected: true, name: "foo" },
          ],
        });
        expect(isFiltered).toBe(true);
        expect(filteredData).toEqual(mockDataState.data);
      });
    });
    describe("charged status", () => {
      beforeEach(() => {
        mockDataState.data = [
          {
            id: 1,
            agency: {},
            area: {},
            offences: [{ isNotYetCharged: true }],
          },
          {
            id: 2,
            agency: {},
            area: {},
            offences: [{ isNotYetCharged: false }],
          },
          {
            id: 3,
            agency: {},
            area: {},
            offences: [{ isNotYetCharged: true }],
          },
          {
            id: 4,
            agency: {},
            area: {},
            offences: [{ isNotYetCharged: false }],
          },
        ] as CaseSearchResult[];
      });

      test("it can filter on charged cases", () => {
        mockFilterState.params.chargedStatus = "true";

        const {
          result: {
            current: { filters, isFiltered, filteredData },
          },
        } = renderHook(() => useSearchState(mockFilterState, mockDataState));

        expect(filters.chargedStatus).toEqual({
          isActive: true,
          name: "chargedStatus",
          items: [
            { count: 2, id: "true", isSelected: true, name: "Charged" },
            { count: 2, id: "false", isSelected: false, name: "Not Charged" },
          ],
        });
        expect(isFiltered).toBe(true);
        expect(filteredData.map((item) => item.id)).toEqual([2, 4]);
      });

      test("it can filter on not charged cases", () => {
        mockFilterState.params.chargedStatus = "false";

        const {
          result: {
            current: { filters, isFiltered, filteredData },
          },
        } = renderHook(() => useSearchState(mockFilterState, mockDataState));

        expect(filters.chargedStatus).toEqual({
          isActive: true,
          name: "chargedStatus",
          items: [
            { count: 2, id: "true", isSelected: false, name: "Charged" },
            { count: 2, id: "false", isSelected: true, name: "Not Charged" },
          ],
        });
        expect(isFiltered).toBe(true);
        expect(filteredData.map((item) => item.id)).toEqual([1, 3]);
      });

      test("it can not filter on charged status", () => {
        mockFilterState.params.chargedStatus = undefined;

        const {
          result: {
            current: { filters, isFiltered, filteredData },
          },
        } = renderHook(() => useSearchState(mockFilterState, mockDataState));

        expect(filters.chargedStatus).toEqual({
          isActive: true,
          name: "chargedStatus",
          items: [
            { count: 2, id: "true", isSelected: false, name: "Charged" },
            {
              count: 2,
              id: "false",
              isSelected: false,
              name: "Not Charged",
            },
          ],
        });
        expect(isFiltered).toBe(true);
        expect(filteredData.map((item) => item.id)).toEqual([1, 2, 3, 4]);
      });
    });
  });

  describe("setting params", () => {
    beforeEach(() => {
      mockDataState.data = [
        { agency: { code: "A" }, area: {} },
        { agency: { code: "B" }, area: {} },
        { agency: { code: "C" }, area: {} },
      ] as CaseSearchResult[];
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

    test("set a single param", () => {
      mockFilterState.params = { urn: "foo", area: ["1"] };

      const {
        result: {
          current: { setFilterSingleParam },
        },
      } = renderHook(() => useSearchState(mockFilterState, mockDataState));

      act(() => setFilterSingleParam("chargedStatus", "true"));

      expect(mockSetParams).toHaveBeenCalledTimes(1);
      expect(mockSetParams).toHaveBeenCalledWith({
        urn: "foo",
        area: ["1"],
        chargedStatus: "true",
      });
    });

    test("set a multiple param and add it to the params", () => {
      mockFilterState.params = { urn: "foo", area: ["1"], agency: ["A"] };

      const {
        result: {
          current: { setFilterMultipleParam },
        },
      } = renderHook(() => useSearchState(mockFilterState, mockDataState));

      act(() => setFilterMultipleParam("agency", "B", true));

      expect(mockSetParams).toHaveBeenCalledTimes(1);
      expect(mockSetParams).toHaveBeenCalledWith({
        urn: "foo",
        area: ["1"],
        agency: ["A", "B"],
      });
    });

    test("set a multiple param of a type so that all options for that type are set", () => {
      mockFilterState.params = { urn: "foo", area: ["1"], agency: ["A", "B"] };

      const {
        result: {
          current: { setFilterMultipleParam },
        },
      } = renderHook(() => useSearchState(mockFilterState, mockDataState));

      act(() => setFilterMultipleParam("agency", "C", true));

      expect(mockSetParams).toHaveBeenCalledTimes(1);
      expect(mockSetParams).toHaveBeenCalledWith({
        urn: "foo",
        area: ["1"],
        agency: undefined,
      });
    });

    test("set all multiple params of a type", () => {
      mockFilterState.params = { urn: "foo", area: ["1"], agency: ["A"] };

      const {
        result: {
          current: { setFilterMultipleParamAll },
        },
      } = renderHook(() => useSearchState(mockFilterState, mockDataState));

      act(() => setFilterMultipleParamAll("agency"));

      expect(mockSetParams).toHaveBeenCalledTimes(1);
      expect(mockSetParams).toHaveBeenCalledWith({
        urn: "foo",
        area: ["1"],
        agency: undefined,
      });
    });

    test("clear all filters", () => {
      mockFilterState.params = { urn: "foo", area: ["1"], agency: ["A"] };

      const {
        result: {
          current: { clearFilters },
        },
      } = renderHook(() => useSearchState(mockFilterState, mockDataState));

      act(() => clearFilters());

      expect(mockSetParams).toHaveBeenCalledTimes(1);
      expect(mockSetParams).toHaveBeenCalledWith({
        urn: "foo",
        area: undefined,
        agency: undefined,
      });
    });
  });
});

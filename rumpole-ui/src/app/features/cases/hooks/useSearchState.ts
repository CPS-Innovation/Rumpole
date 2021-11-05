import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "../../../common/redux/hooks";
import { CaseSearchResult } from "../domain/CaseSearchResult";
import { clearCases, fetchCases, selectAll } from "../redux/casesSlice";
import { CaseFilterQueryParams } from "../types/CaseFilterQueryParams";
import { useQueryParams } from "./useQueryParams";

const FILTER_COUNT_VISIBLE_THRESHOLD = 2;

type FilterPropertyNames = keyof Omit<CaseFilterQueryParams, "urn">;

export type FilterDetails = {
  name: FilterPropertyNames;
  items: {
    [key: string]: {
      name: string;
      count: number;
      isSelected: boolean;
    };
  };
  isActive: boolean;
};

type Filters = Record<FilterPropertyNames, FilterDetails>;

export type SearchState = ReturnType<typeof useSearchState>;

export const useSearchState = () => {
  const { setParams, params } = useQueryParams<CaseFilterQueryParams>();

  const data = useAppSelector(selectAll);
  const { status: loadingStatus, urn: reduxUrn } = useAppSelector(
    (state) => state.cases
  );

  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(params.urn ? fetchCases(params.urn) : clearCases());
  }, [dispatch, params.urn]);

  const totalCount = data.length;

  const filters: Filters = {
    area: buildFilterDetails({
      filterName: "area",
      data,
      sortFn: (a, b) => (a.area.name > b.area.name ? 1 : -1),
      valuesFn: (item) => ({
        ...item.area,
        isSelected:
          params.area === undefined || params.area.includes(item.area.code),
      }),
    }),
    agency: buildFilterDetails({
      filterName: "agency",
      data,
      sortFn: (a, b) => (a.agency.name > b.agency.name ? 1 : -1),
      valuesFn: (item) => ({
        ...item.agency,
        isSelected:
          params.agency === undefined ||
          params.agency.includes(item.agency.code),
      }),
    }),
    status: buildFilterDetails({
      filterName: "status",
      data,
      sortFn: (a, b) => {
        const sortOrderings = ["O", "F", "C", "N"];
        return sortOrderings.indexOf(a.status.code) >
          sortOrderings.indexOf(b.status.code)
          ? 1
          : -1;
      },
      valuesFn: (item) => ({
        code: item.status.code,
        name: item.status.description,
        isSelected: params.status === item.status.code,
      }),
    }),
  };

  const filteredData = data.filter(
    (item) =>
      filters.area.items[item.area.code].isSelected &&
      filters.agency.items[item.agency.code].isSelected &&
      (filters.status.items[item.status.code].isSelected ||
        !Object.values(filters.status.items).some((item) => item.isSelected))
  );

  const setUrnParam = (urn: string) => setParams({ urn });

  const setFilterSingleParam = (
    name: FilterPropertyNames,
    value: string | undefined
  ) => setParams({ ...params, [name]: value });

  const setFilterMultipleParam = (
    name: FilterPropertyNames,
    value: string,
    isSelected: boolean
  ) => {
    const previousValues = Object.entries(filters[name].items)
      .filter(([, value]) => value.isSelected)
      .map(([key]) => key);

    const nextValues = isSelected
      ? [...previousValues, value]
      : previousValues.filter((previousValue) => previousValue !== value);

    const isEveryValueSet =
      nextValues.length === Object.keys(filters[name].items).length;

    setParams({
      ...params,
      [name]: isEveryValueSet ? undefined : nextValues,
    });
  };

  const setFilterMultipleParamAll = (name: FilterPropertyNames) => {
    setParams({
      ...params,
      [name]: undefined,
    });
  };

  return {
    loadingStatus: params.urn !== reduxUrn ? "idle" : loadingStatus,
    totalCount,
    filters,
    filteredData,
    // todo: this is quite a big footprint of differnt methods to expose, could it be made more concise
    setUrnParam,
    setFilterSingleParam,
    setFilterMultipleParam,
    setFilterMultipleParamAll,
    urn: params.urn,
  };
};

const buildFilterDetails = ({
  filterName,
  data,
  sortFn,
  valuesFn,
}: {
  filterName: FilterPropertyNames;
  data: CaseSearchResult[];
  sortFn: (a: CaseSearchResult, b: CaseSearchResult) => number;
  valuesFn: (item: CaseSearchResult) => {
    code: string;
    name: string;
    isSelected: boolean;
  };
}): FilterDetails => {
  const items = data.sort(sortFn).reduce((accumulator, item) => {
    const { code, name, isSelected } = valuesFn(item);

    accumulator[code] = {
      name,
      count: (accumulator[code]?.count ?? 0) + 1,
      isSelected,
    };

    return accumulator;
  }, {} as NonNullable<FilterDetails["items"]>);
  return {
    name: filterName,
    items,
    isActive: Object.keys(items).length >= FILTER_COUNT_VISIBLE_THRESHOLD,
  };
};

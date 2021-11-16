import { CaseSearchResult } from "../domain/CaseSearchResult";

import { CaseFilterQueryParams } from "./types/CaseFilterQueryParams";
import { QueryParamsState } from "./useQueryParams";
import { SearchDataState } from "./useSearchDataState";

const FILTER_COUNT_VISIBLE_THRESHOLD = 1;

type FilterPropertyName = keyof Omit<CaseFilterQueryParams, "urn">;

export type FilterDetails = {
  name: FilterPropertyName;
  items: {
    id: string;
    name: string;
    count: number;
    isSelected: boolean;
  }[];

  isActive: boolean;
};

type Filters = Record<FilterPropertyName, FilterDetails>;

export type SearchState = ReturnType<typeof useSearchState>;

export const useSearchState = (
  { setParams, params }: QueryParamsState<CaseFilterQueryParams>,
  { data, reduxUrn, loadingStatus }: SearchDataState
) => {
  const { urn, ...filterParams } = params;

  const totalCount = data.length;

  const filters: Filters = {
    area: buildFilterDetails({
      filterName: "area",
      data,
      sortFn: (a, b) => (a.area.name > b.area.name ? 1 : -1),
      valuesFn: (item) => ({
        id: item.area.code,
        name: item.area.name,
        isSelected:
          filterParams.area === undefined ||
          filterParams.area.includes(item.area.code),
      }),
    }),
    agency: buildFilterDetails({
      filterName: "agency",
      data,
      sortFn: (a, b) => (a.agency.name > b.agency.name ? 1 : -1),
      valuesFn: (item) => ({
        id: item.agency.code,
        name: item.agency.name,
        isSelected:
          filterParams.agency === undefined ||
          filterParams.agency.includes(item.agency.code),
      }),
    }),
    chargedStatus: buildFilterDetails({
      filterName: "chargedStatus",
      data,
      sortFn: (a, b) => (isCharged(a) > isCharged(b) ? -1 : 1),
      valuesFn: (item) => {
        const id = isCharged(item) ? "true" : "false";
        return {
          id,
          name: isCharged(item) ? "Charged" : "Not Charged",
          isSelected: filterParams.chargedStatus === id,
        };
      },
    }),
  };

  const filteredData = data.filter(
    (item) =>
      filters.area.items.find((filterItem) => filterItem.id === item.area.code)
        ?.isSelected &&
      filters.agency.items.find(
        (filterItem) => filterItem.id === item.agency.code
      )?.isSelected &&
      (filters.chargedStatus.items.find(
        (filterItem) => filterItem.id === (isCharged(item) ? "true" : "false")
      )?.isSelected ||
        !filters.chargedStatus.items.some((item) => item.isSelected))
  );

  const setUrnParam = (urn: string) => setParams({ urn });

  const setFilterSingleParam = (
    name: FilterPropertyName,
    value: string | undefined
  ) => setParams({ ...params, [name]: value });

  const setFilterMultipleParam = (
    name: FilterPropertyName,
    value: string,
    isSelected: boolean
  ) => {
    const previousValues = filters[name].items
      .filter((item) => item.isSelected)
      .map((item) => item.id);

    const nextValues = isSelected
      ? [...previousValues, value]
      : previousValues.filter((previousValue) => previousValue !== value);

    const isEveryValueSet = nextValues.length === filters[name].items.length;

    setParams({
      ...params,
      [name]: isEveryValueSet ? undefined : nextValues,
    });
  };

  const setFilterMultipleParamAll = (name: FilterPropertyName) => {
    setParams({
      ...params,
      [name]: undefined,
    });
  };

  const clearFilters = () => setParams({ urn });

  return {
    urn,
    loadingStatus: urn !== reduxUrn ? "idle" : loadingStatus,
    totalCount,
    filters,
    filteredData,
    isFiltered: Object.keys(filterParams).length > 0,
    setUrnParam,
    setFilterSingleParam,
    setFilterMultipleParam,
    setFilterMultipleParamAll,
    clearFilters,
  };
};

const buildFilterDetails = ({
  filterName,
  data,
  sortFn,
  valuesFn,
}: {
  filterName: FilterPropertyName;
  data: CaseSearchResult[];
  sortFn: (a: CaseSearchResult, b: CaseSearchResult) => number;
  valuesFn: (item: CaseSearchResult) => {
    id: string;
    name: string;
    isSelected: boolean;
  };
}): FilterDetails => {
  const items = [...data].sort(sortFn).reduce((accumulator, item) => {
    const { id, name, isSelected } = valuesFn(item);
    if (!accumulator.some((existingItem) => existingItem.id === id)) {
      accumulator.push({ id, name, count: 0, isSelected: false });
    }
    var thisItem = accumulator[accumulator.length - 1];
    thisItem.count += 1;
    thisItem.isSelected = isSelected;

    return accumulator;
  }, [] as FilterDetails["items"]);
  return {
    name: filterName,
    items,
    isActive: items.length >= FILTER_COUNT_VISIBLE_THRESHOLD,
  };
};

const isCharged = (result: CaseSearchResult) =>
  result.offences?.some((item) => !item.isNotYetCharged);

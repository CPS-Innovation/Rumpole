import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "../../../redux/hooks";
import { CaseSearchResult } from "../domain/CaseSearchResult";
import { fetchCases, selectAll } from "../redux/casesSlice";
import { CaseFilterQueryParams } from "../types/CaseFilterQueryParams";
import { useQueryParams } from "./useQueryParams";

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

const buildFilterDetails = (
  filterName: FilterPropertyNames,
  state: CaseSearchResult[],
  getValues: (item: CaseSearchResult) => {
    code: string;
    name: string;
    isSelected: boolean;
  }
): FilterDetails => {
  const items = state.reduce((accumulator, item) => {
    const { code, name, isSelected } = getValues(item);

    accumulator[code] = {
      name,
      count: (accumulator[code]?.count ?? 0) + 1,
      isSelected,
    };

    return accumulator;
  }, {} as NonNullable<FilterDetails["items"]>);
  return { name: filterName, items, isActive: Object.keys(items).length > 1 };
};

export const useSearchState = () => {
  const { setParams, params } = useQueryParams<CaseFilterQueryParams>();
  const data = useAppSelector(selectAll);

  const dispatch = useAppDispatch();

  useEffect(() => {
    params.urn && dispatch(fetchCases(params.urn));
  }, [dispatch, params.urn]);

  const totalCount = data.length;

  const filters: Filters = {
    area: buildFilterDetails("area", data, (item) => ({
      ...item.area,
      isSelected:
        params.area === undefined || params.area.includes(item.area.code),
    })),
    agency: buildFilterDetails("agency", data, (item) => ({
      ...item.agency,
      isSelected:
        params.agency === undefined || params.agency.includes(item.agency.code),
    })),
    status: buildFilterDetails("status", data, (item) => ({
      code: item.status.code,
      name: item.status.description,
      isSelected:
        params.status === undefined || params.status === item.status.code,
    })),
  };

  const filteredData = data.filter(
    (item) =>
      filters.area.items[item.area.code].isSelected &&
      filters.agency.items[item.agency.code].isSelected &&
      filters.status.items[item.status.code].isSelected
  );

  const setUrnParam = (urn: string) => setParams({ urn });

  const setFilterParam = (
    name: FilterPropertyNames,
    value: string,
    isSelected: boolean
  ) => {
    const previousValues = Object.entries(filters[name].items)
      .filter(([_, value]) => value.isSelected)
      .map(([key]) => key);

    const nextValues = isSelected
      ? [...previousValues, value]
      : previousValues.filter((previousValue) => previousValue !== value);

    const isEveryValueSet =
      nextValues.length === Object.keys(filters[name].items).length;

    setParams({ ...params, [name]: isEveryValueSet ? undefined : nextValues });
  };

  return {
    totalCount,
    filters,
    filteredData,
    setUrnParam,
    setFilterParam,
    urn: params.urn,
  };
};

export type SearchState = ReturnType<typeof useSearchState>;

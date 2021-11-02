import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "../../../redux/hooks";
import { CaseSearchResult } from "../domain/CaseSearchResult";
import { fetchCases, selectAll } from "../redux/casesSlice";
import { CaseFilterQueryParams } from "../types/CaseFilterQueryParams";
import { useQueryParams } from "./useQueryParams";

export type StringFilterType = string | undefined;
export type ArrayFiltertype = string[] | undefined;

export type FilterDetails<T extends StringFilterType | ArrayFiltertype> = {
  [key: string]: {
    name: string;
    count: number;
    selected: T;
  };
};

type Filters = {
  area: FilterDetails<ArrayFiltertype> | undefined;
  agency: FilterDetails<ArrayFiltertype> | undefined;
  status: FilterDetails<StringFilterType> | undefined;
};

const buildFilterDetails = <T extends StringFilterType | ArrayFiltertype>(
  state: CaseSearchResult[],
  getfilterItemKeyAndName: (item: CaseSearchResult) => {
    code: string;
    name: string;
  },
  getSelected: () => T
) => {
  const result = state.reduce((accumulator, item) => {
    const { code, name } = getfilterItemKeyAndName(item);
    const selected = getSelected();
    accumulator[code] = {
      name,
      count: (accumulator[code]?.count ?? 0) + 1,
      selected,
    };
    return accumulator;
  }, {} as FilterDetails<T>);

  return Object.keys(result).length > 1 ? result : undefined;
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
    area: buildFilterDetails(
      data,
      (item) => ({
        ...item.area,
      }),
      () => params.area
    ),
    agency: buildFilterDetails(
      data,
      (item) => ({
        ...item.agency,
      }),
      () => params.agency
    ),
    status: buildFilterDetails(
      data,
      (item) => ({
        code: item.status.code,
        name: item.status.description,
      }),
      () => params.status
    ),
  };

  const filteredData = data.filter(
    (item) =>
      (params.area === undefined || params.area.includes(item.area.code)) &&
      (params.agency === undefined ||
        params.agency.includes(item.agency.code)) &&
      (params.status === undefined || params.status === item.status.code)
  );

  return { totalCount, filters, filteredData, setParams, params };
};

export type SearchState = ReturnType<typeof useSearchState>;

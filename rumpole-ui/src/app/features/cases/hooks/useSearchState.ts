import { CaseSearchResult } from "../domain/CaseSearchResult";

import { CaseFilterQueryParams } from "./types/CaseFilterQueryParams";
import { QueryParamsState } from "../../../common/hooks/useQueryParamsState";
import { SearchDataState } from "./useSearchDataState";

export type FilterDetails = {
  items: {
    id: string;
    text: string;
    count: number;
    isSelected: boolean;
  }[];
};

export type SearchState = ReturnType<typeof useSearchState>;

export const useSearchState = (
  { setParams, params }: QueryParamsState<CaseFilterQueryParams>,
  { data, reduxUrn, loadingStatus, error }: SearchDataState
) => {
  const { urn, chargedStatus } = params;
  const totalCount = data.length;

  const filterItems = [
    { value: "", text: "All", count: totalCount },
    {
      value: "true",
      text: "Charged",
      count: data.filter((item) => isCharged(item)).length,
    },
    {
      value: "false",
      text: "Not yet charged",
      count: data.filter((item) => !isCharged(item)).length,
    },
  ].map((item) => ({ ...item, text: `${item.text} (${item.count})` }));

  const filteredData = chargedStatus
    ? data.filter((item) => (chargedStatus === "true") === isCharged(item))
    : data;

  const setUrnParam = (urn: string) => setParams({ urn });

  const setChargedStatus = (value: string | undefined) =>
    setParams({
      ...params,
      chargedStatus: (value ||
        undefined) as CaseFilterQueryParams["chargedStatus"],
    });

  return {
    totalCount,
    loadingStatus: urn !== reduxUrn ? "idle" : loadingStatus,

    urn,
    chargedStatus: chargedStatus || "",

    filterItems,
    filteredData,

    setUrnParam,
    setChargedStatus,

    error,
  };
};

const isCharged = (result: CaseSearchResult) =>
  result.offences?.some((item) => !item.isNotYetCharged);

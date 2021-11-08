import { FC } from "react";
import { SearchState } from "../../hooks/useSearchState";
import { FilterMultiple } from "./FilterMultiple";
import { FilterSingle } from "./FilterSinlge";

type FiltersProps = {
  searchState: SearchState;
};

export const Filters: FC<FiltersProps> = ({
  searchState: {
    filters: { area, agency, status, chargedStatus },
    setFilterSingleParam,
    setFilterMultipleParam,
    setFilterMultipleParamAll,
    loadingStatus,
  },
}) => {
  if (loadingStatus !== "succeeded") return null;

  return (
    <>
      {chargedStatus.isActive && (
        <FilterSingle
          title="Charged Status"
          filterDetails={chargedStatus}
          setFilterParam={setFilterSingleParam}
        />
      )}
      {area.isActive && (
        <FilterMultiple
          title="Area"
          filterDetails={area}
          setFilterParam={setFilterMultipleParam}
          setFilterParamAll={setFilterMultipleParamAll}
        />
      )}
      {agency.isActive && (
        <FilterMultiple
          title="Agency"
          filterDetails={agency}
          setFilterParam={setFilterMultipleParam}
          setFilterParamAll={setFilterMultipleParamAll}
        />
      )}
    </>
  );
};

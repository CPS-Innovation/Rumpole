import { FC } from "react";
import { SearchState } from "../../hooks/useSearchState";
import { FilterMultiple } from "./FilterMultiple";
import { FilterSingle } from "./FilterSinlge";

type FiltersProps = {
  searchState: SearchState;
};

export const Filters: FC<FiltersProps> = ({
  searchState: {
    filters: { area, agency, status },
    setFilterSingleParam,
    setFilterMultipleParam,
    setFilterMultipleParamAll,
    loadingStatus,
  },
}) => {
  if (loadingStatus !== "succeeded") return null;

  return (
    <>
      {area.isActive && (
        <FilterMultiple
          title="Area"
          filterDetails={area}
          setFilterParam={setFilterMultipleParam}
          setFilterParamAll={setFilterMultipleParamAll}
        />
      )}
      {status.isActive && (
        <FilterSingle
          title="Case Status"
          filterDetails={status}
          setFilterParam={setFilterSingleParam}
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

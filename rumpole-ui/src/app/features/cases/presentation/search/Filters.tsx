import { FC } from "react";
import { SearchState } from "../../hooks/useSearchState";
import { FilterMany } from "./FilterMany";

type FiltersProps = {
  searchState: SearchState;
};

export const Filters: FC<FiltersProps> = ({
  searchState: {
    filters: { area, agency, status },
    setFilterParam,
  },
}) => {
  return (
    <>
      {area.isActive && (
        <FilterMany
          title="Area"
          filterDetails={area}
          setFilterParam={setFilterParam}
        />
      )}
      {agency.isActive && (
        <FilterMany
          title="Agency"
          filterDetails={agency}
          setFilterParam={setFilterParam}
        />
      )}
    </>
  );
};

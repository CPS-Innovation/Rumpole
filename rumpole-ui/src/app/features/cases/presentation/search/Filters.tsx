import { FC } from "react";
import { SearchState } from "../../hooks/useSearchState";
import { FilterMany } from "./FilterMany";

type FiltersProps = {
  searchState: SearchState;
};

export const Filters: FC<FiltersProps> = ({
  searchState: {
    filters: { area, agency, status },
  },
}) => {
  return (
    <>
      {area && <FilterMany title="Area" filterDetails={area} />}
      {agency && <FilterMany title="Agency" filterDetails={agency} />}
    </>
  );
};

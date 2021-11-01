import { FC } from "react";

import { useSearch } from "../../hooks/useSearch";
import { UrnFilterQueryParams } from "../../types/UrnFilterQueryParams";
import { Results } from "./Results";
import { SearchBar } from "./SearchBar";

export const Page: FC = () => {
  const { handleSearch, handleFilter, urn } = useSearch<UrnFilterQueryParams>();

  return (
    <>
      <SearchBar onSearch={handleSearch} initialUrn={urn} />
      <Results onFilter={handleFilter} />
    </>
  );
};

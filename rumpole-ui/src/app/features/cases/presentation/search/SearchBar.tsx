import { Search } from "@mui/icons-material";
import { Button } from "@mui/material";
import { FC, useState } from "react";
import { SearchState } from "../../hooks/useSearchState";
import { isUrnValid } from "../../logic/isUrnValid";

import { SearchUrnField } from "./SearchUrnField";

type SearchBarProps = {
  searchState: SearchState;
};

export const SearchBar: FC<SearchBarProps> = ({
  searchState: { params, setParams },
}) => {
  const [urn, setUrn] = useState(params.urn || "");
  const isValid = isUrnValid(urn);

  const handleChange = (val: string) => setUrn(val.toUpperCase());

  const handleSubmit = () => {
    isValid && setParams({ urn });
  };

  return (
    <>
      <Search fontSize="large" />

      <SearchUrnField
        value={urn}
        onChange={handleChange}
        onSubmit={handleSubmit}
      />

      <Button
        variant="contained"
        color="secondary"
        onClick={handleSubmit}
        disabled={!isValid}
        size="large"
      >
        Search
      </Button>
    </>
  );
};

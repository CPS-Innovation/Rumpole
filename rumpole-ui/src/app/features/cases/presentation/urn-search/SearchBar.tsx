import { Search } from "@mui/icons-material";
import { Button, Container, Paper } from "@mui/material";
import { FC, useState } from "react";
import { isUrnValid } from "../../logic/isUrnValid";
import { SearchUrnField } from "./SearchUrnField";

type SearchBarProps = {
  onSearch: (urn: string) => void;
  initialUrn: string | undefined;
};

export const SearchBar: FC<SearchBarProps> = ({ onSearch, initialUrn }) => {
  const [urn, setUrn] = useState(initialUrn || "");
  const isValid = isUrnValid(urn);

  const handleChange = (val: string) => setUrn(val.toUpperCase());

  const handleSubmit = () => {
    isValid && onSearch(urn);
  };

  return (
    <Paper>
      <Container
        sx={{
          height: 60,
          display: "flex",
          alignItems: "center",
        }}
      >
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
        >
          Search
        </Button>
      </Container>
    </Paper>
  );
};

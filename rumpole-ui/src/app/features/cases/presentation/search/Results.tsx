import { Box } from "@mui/system";
import { FC } from "react";

import { SearchState } from "../../hooks/useSearchState";
import { Header } from "./Header";
import { Result } from "./Result";

type ResultsProps = {
  searchState: SearchState;
};
export const Results: FC<ResultsProps> = ({
  searchState: { filteredData, totalCount, urn },
}) => {
  const isFiltered = filteredData.length !== totalCount;

  const summary = isFiltered
    ? `Showing ${filteredData.length} of ${totalCount} results for URN: ${urn}`
    : `${totalCount} result${totalCount > 1 ? "s" : ""} for URN: ${urn}`;

  return (
    <>
      <Header>{summary}</Header>
      <Box role="list">
        {filteredData.map((item) => (
          <Result result={item} key={item.id} />
        ))}
      </Box>
    </>
  );
};

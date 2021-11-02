import { Box } from "@mui/system";
import { FC } from "react";

import { SearchState } from "../../hooks/useSearchState";
import { Header } from "./Header";
import { Result } from "./Result";

type ResultsProps = {
  searchState: SearchState;
};
export const Results: FC<ResultsProps> = ({
  searchState: {
    filteredData,
    totalCount,
    params: { urn },
  },
}) => {
  return (
    <>
      <Header>
        {totalCount} results for URN: {urn}
      </Header>
      <Box role="list">
        {filteredData.map((item) => (
          <Result result={item} key={item.id} />
        ))}
      </Box>
    </>
  );
};

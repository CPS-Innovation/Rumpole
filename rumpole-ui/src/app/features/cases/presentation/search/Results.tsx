import { Box } from "@mui/system";
import { FC } from "react";

import { SearchState } from "../../hooks/useSearchState";
import { Result } from "./Result";

type ResultsProps = {
  searchState: SearchState;
};
export const Results: FC<ResultsProps> = ({
  searchState: { filteredData, loadingStatus },
}) => {
  return (
    <>
      {loadingStatus === "succeeded" && (
        <Box role="list" data-testid="element-results">
          {filteredData.map((item) => (
            <Result result={item} key={item.id} />
          ))}
        </Box>
      )}
    </>
  );
};

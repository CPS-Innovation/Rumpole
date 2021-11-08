import { Box } from "@mui/material";
import { FC } from "react";
import { InlineButton } from "../../../../common/presentation/components/InlineButton";
import { SearchState } from "../../hooks/useSearchState";
import { Header } from "./Header";

type ResultsTitleProps = {
  searchState: SearchState;
};
export const ResultsTitle: FC<ResultsTitleProps> = ({
  searchState: {
    filteredData,
    totalCount,
    urn,
    loadingStatus,
    isFiltered,
    clearFilters,
  },
}) => {
  let summary: string;

  if (loadingStatus !== "succeeded") {
    summary = "Please wait...";
  } else if (filteredData.length !== totalCount) {
    summary = `Showing ${filteredData.length} of ${totalCount} result${
      filteredData.length === 1 ? "" : "s"
    } for URN: ${urn}`;
  } else {
    summary = `Showing ${totalCount} result${
      totalCount === 1 ? "" : "s"
    } for URN: ${urn}`;
  }

  return (
    <Header>
      <Box sx={{ flexGrow: 1 }}>{summary}</Box>
      {isFiltered && (
        <Box sx={{ alignSelf: "center" }}>
          &nbsp;&nbsp;&nbsp;&nbsp;
          <InlineButton onClick={clearFilters}>Clear Filters</InlineButton>
        </Box>
      )}
    </Header>
  );
};

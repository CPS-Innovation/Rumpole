import { FC } from "react";
import { SearchState } from "../../hooks/useSearchState";
import { Header } from "./Header";

type ResultsTitleProps = {
  searchState: SearchState;
};
export const ResultsTitle: FC<ResultsTitleProps> = ({
  searchState: { filteredData, totalCount, urn, loadingStatus },
}) => {
  let summary: string;

  if (loadingStatus !== "succeeded") {
    summary = "Please wait...";
  } else if (filteredData.length !== totalCount) {
    summary = `Showing ${filteredData.length} of ${totalCount} result${
      filteredData.length > 1 ? "s" : ""
    } for URN: ${urn}`;
  } else {
    summary = `Showing ${totalCount} result${
      totalCount > 1 ? "s" : ""
    } for URN: ${urn}`;
  }

  return <Header>{summary}</Header>;
};

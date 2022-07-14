import React from "react";
import { CaseDetailsState } from "../../../hooks/use-case-details-state/useCaseDetailsState";
import { EmptySearchTerm } from "./not-ready-modes/EmptySearchTerm";
import { PipelineNotReady } from "./not-ready-modes/PipelineNotReady";
import { SearchInProgress } from "./not-ready-modes/SearchInProgress";
import { Results } from "./ready-mode/Results";

type Props = {
  pipelineState: CaseDetailsState["pipelineState"];
  searchState: CaseDetailsState["searchState"];
  handleChangeResultsOrder: CaseDetailsState["handleChangeResultsOrder"];
  handleUpdateFilter: CaseDetailsState["handleUpdateFilter"];
  handleOpenPdf: CaseDetailsState["handleOpenPdf"];
};

const MemoizedResults = React.memo(Results);

export const ContentModeSwitch: React.FC<Props> = ({
  pipelineState: { status },
  searchState: {
    results,
    submittedSearchTerm,
    missingDocs,
    resultsOrder,
    filterOptions,
  },
  handleChangeResultsOrder,
  handleUpdateFilter,
  handleOpenPdf,
}) => {
  if (!submittedSearchTerm) {
    return <EmptySearchTerm />;
  }

  if (status !== "complete") {
    return <PipelineNotReady />;
  }

  if (results.status === "loading") {
    return <SearchInProgress />;
  }

  const { data: searchResult } = results;

  return (
    <MemoizedResults
      {...{
        missingDocs,
        searchResult,
        submittedSearchTerm,
        resultsOrder,
        handleChangeResultsOrder,
        filterOptions,
        handleUpdateFilter,
        handleOpenPdf,
      }}
    />
  );
};

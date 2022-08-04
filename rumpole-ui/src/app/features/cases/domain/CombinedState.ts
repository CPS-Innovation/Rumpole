import { AsyncPipelineResult } from "../hooks/use-pipeline-api/AsyncPipelineResult";
import { AsyncResult } from "../../../common/types/AsyncResult";
import { CaseDetails } from "./CaseDetails";

import { CaseDocumentViewModel } from "./CaseDocumentViewModel";
import { PipelineResults } from "./PipelineResults";
import { MappedTextSearchResult } from "./MappedTextSearchResult";
import { AccordionDocumentSection } from "../presentation/case-details/accordion/types";
import { MappedCaseDocument } from "./MappedCaseDocument";
import { FilterOption } from "./FilterOption";

export type CombinedState = {
  caseState: AsyncResult<CaseDetails>;
  documentsState: AsyncResult<MappedCaseDocument[]>;
  pipelineState: AsyncPipelineResult<PipelineResults>;
  accordionState: AsyncResult<AccordionDocumentSection[]>;
  tabsState: {
    items: CaseDocumentViewModel[];
    authToken: undefined | string;
  };
  // `searchTerm` is outside of `searchState` as it is more volatile: when
  //  the user is typing away, we prevent `searchState` from continually being
  //  rebuilt and avoiding rerenders caused by continual props reference changes.
  searchTerm: string;
  searchState: {
    isResultsVisible: boolean;
    submittedSearchTerm: undefined | string;
    resultsOrder: "byDateDesc" | "byOccurancesPerDocumentDesc";
    filterOptions: {
      docType: { [key: string]: FilterOption };
      category: { [key: string]: FilterOption };
    };
    missingDocs: { documentId: string; fileName: string }[];
    results: AsyncResult<MappedTextSearchResult>;
  };
};
import { DelayedResult } from "../../../../common/types/DelayedResult";
import { CaseDetails } from "../../domain/CaseDetails";
import { CaseDocument } from "../../domain/CaseDocument";
import { CaseDocumentWithUrl } from "../../domain/CaseDocumentWithUrl";
import { PipelineResults } from "../../domain/PipelineResults";
import { AccordionDocumentSection } from "../../presentation/case-details/accordion/types";

export type CombinedState = {
  caseState: DelayedResult<CaseDetails>;
  documentsState: DelayedResult<CaseDocument[]>;
  pipelineState: DelayedResult<PipelineResults>;
  accordionState: DelayedResult<AccordionDocumentSection[]>;
  tabsState: {
    items: CaseDocumentWithUrl[];
    authToken: undefined | string;
  };
  searchState: {
    isResultsVisible: boolean;
    searchTerm: undefined | string;
  };
};

import { CaseDocumentWithTabSafeId } from "../../../domain/CaseDocumentWithTabSafeId";

export type AccordionDocumentSection = {
  sectionId: string;
  sectionLabel: string;
  docs: CaseDocumentWithTabSafeId[];
};

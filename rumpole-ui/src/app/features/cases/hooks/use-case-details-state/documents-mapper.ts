import { ApiResult } from "../../../../common/types/ApiResult";
import { DelayedResult } from "../../../../common/types/DelayedResult";
import { CaseDocument } from "../../domain/CaseDocument";
import { AccordionDocumentSection } from "../../presentation/case-details/accordion/types";

const sectionTestersInPresentationOrder: {
  sectionId: string;
  showIfEmpty: boolean;
  test: (caseDocument: CaseDocument) => boolean;
}[] = [
  // todo: when we know, write the `test` logic to identify which document goes in which section
  { sectionId: "Reviews", showIfEmpty: true, test: () => false },
  { sectionId: "Case overview", showIfEmpty: true, test: () => false },
  { sectionId: "Statements", showIfEmpty: true, test: () => false },
  { sectionId: "Exhibits", showIfEmpty: true, test: () => false },
  { sectionId: "Forensics", showIfEmpty: true, test: () => false },
  { sectionId: "Unused materials", showIfEmpty: true, test: () => false },
  { sectionId: "Defendant", showIfEmpty: true, test: () => false },
  { sectionId: "Court preparation", showIfEmpty: true, test: () => false },
  { sectionId: "Communications", showIfEmpty: true, test: () => false },
  // have unknown last so it can scoop up any unmatched documents
  { sectionId: "Uncategorised", showIfEmpty: false, test: () => true },
];

export const mapDocumentsToAccordionState = (
  documentsState: ApiResult<CaseDocument[]>
): DelayedResult<AccordionDocumentSection[]> => {
  if (documentsState.status !== "succeeded") {
    // we wait for documentsState to be ready, even if pipeline results arrive first
    return {
      status: "loading",
    };
  }

  // first make sure we have every section represented in our results
  //  (we want to return sections even if they are empty)
  const results: AccordionDocumentSection[] =
    sectionTestersInPresentationOrder.map(({ sectionId }) => ({
      sectionId: sectionId,
      sectionLabel: sectionId,
      docs: [],
    }));

  // cycle through each doc from the api
  for (const caseDocument of documentsState.data) {
    // for each doc step through the ordered list of testers ...
    for (const documentTester of sectionTestersInPresentationOrder) {
      // ... until we find one that matches our document ...
      if (documentTester.test(caseDocument)) {
        const resultItem = results.find(
          (item) => item.sectionId === documentTester.sectionId
        )!;

        // ... add to the section results ...
        resultItem.docs.push({
          ...caseDocument,
          tabSafeId: `d${resultItem.docs.length}`,
        });

        // ... and no need to apply any more tests to this document
        break;
      }
    }
  }

  const visibleResults = results.filter(
    (item) =>
      // a section is shown if it contains at least one document...
      item.docs.length ||
      // ... or is set to be visible even if empty
      sectionTestersInPresentationOrder.some(
        (sectionDefinition) =>
          sectionDefinition.sectionId === item.sectionId &&
          sectionDefinition.showIfEmpty
      )
  );

  return { status: "succeeded", data: visibleResults };
};
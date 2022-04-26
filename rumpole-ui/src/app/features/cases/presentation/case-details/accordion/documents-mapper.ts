import { UseApiResult } from "../../../../../common/hooks/useApi";
import { CaseDocument } from "../../../domain/CaseDocument";
import { AccordionDocument, Section } from "./types";

const sectionTestersInPresentationOrder: {
  sectionId: string;
  test: (caseDocument: CaseDocument) => boolean;
}[] = [
  // todo: when we know, write the `test` logic to identify which document goes in which section
  { sectionId: "Reviews", test: () => false },
  { sectionId: "Case overview", test: () => false },
  { sectionId: "Statements", test: () => false },
  { sectionId: "Exhibits", test: () => false },
  { sectionId: "Forensics", test: () => false },
  { sectionId: "Unused materials", test: () => false },
  { sectionId: "Defendant", test: () => false },
  { sectionId: "Court preparation", test: () => false },
  { sectionId: "Communications", test: () => false },
  // have unknown last so it can scoop up any unmatched documents
  { sectionId: "Unknown", test: () => true },
];

export const documentsMapper = (
  documentsState: UseApiResult<CaseDocument[]>
): Section[] | false => {
  if (documentsState.status !== "succeeded") {
    return false;
  }
  // first make sure we have every section represented in our results
  //  (we want to return sections even if they are empty)
  const results: Section[] = sectionTestersInPresentationOrder.map(
    ({ sectionId }) => ({
      sectionId: sectionId,
      sectionLabel: sectionId,
      docs: [],
    })
  );

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
          docId: caseDocument.id,
          docLabel: caseDocument.name,
          docDate: caseDocument.isoDate,
        } as AccordionDocument);

        // ... and no need to apply any more tests to this document
        break;
      }
    }
  }

  return results;
};

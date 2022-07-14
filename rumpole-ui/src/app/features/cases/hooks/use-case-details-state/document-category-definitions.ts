import { CaseDocument } from "../../domain/CaseDocument";

const documentCategoryDefinitions: {
  category: string;
  showIfEmpty: boolean;
  test: (caseDocument: CaseDocument) => boolean;
}[] = [
  // todo: when we know, write the `test` logic to identify which document goes in which section
  { category: "Reviews", showIfEmpty: true, test: (caseDocument) => false },
  {
    category: "Case overview",
    showIfEmpty: true,
    test: (caseDocument) => false,
  },
  {
    category: "Statements",
    showIfEmpty: true,
    test: (caseDocument) => false,
  },
  { category: "Exhibits", showIfEmpty: true, test: (caseDocument) => false },
  { category: "Forensics", showIfEmpty: true, test: (caseDocument) => false },
  {
    category: "Unused materials",
    showIfEmpty: true,
    test: (caseDocument) => false,
  },
  { category: "Defendant", showIfEmpty: true, test: (caseDocument) => false },
  {
    category: "Court preparation",
    showIfEmpty: true,
    test: (caseDocument) => false,
  },
  {
    category: "Communications",
    showIfEmpty: true,
    test: (caseDocument) => false,
  },
  // have unknown last so it can scoop up any unmatched documents
  {
    category: "Uncategorised",
    showIfEmpty: false,
    test: (caseDocument) => true,
  },
];

export const categoryNamesInPresentationOrder = documentCategoryDefinitions.map(
  ({ category }) => category
);

export const getCategory = (item: CaseDocument) =>
  documentCategoryDefinitions.find(({ test }) => test(item))!.category;

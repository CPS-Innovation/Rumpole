import { CaseDocument } from "../../domain/CaseDocument";

const documentCategoryDefinitions: {
  category: string;
  showIfEmpty: boolean;
  test: (caseDocument: CaseDocument) => boolean;
}[] = [
  // todo: when we know, write the `test` logic to identify which document goes in which section
  {
    category: "Reviews",
    showIfEmpty: true,
    test: ({ cmsDocType: { code } }) => ["MG3", "MG3A"].includes(code),
  },
  {
    category: "Case overview",
    showIfEmpty: true,
    test: ({ cmsDocType: { code } }) => ["MG4", "MG5", "MG6"].includes(code),
  },
  {
    category: "Statements",
    showIfEmpty: true,
    test: ({ cmsDocType: { code } }) => ["MG9", "MG11", "PE1"].includes(code),
  },
  {
    category: "Exhibits",
    showIfEmpty: true,
    test: ({ cmsDocType: { code } }) =>
      ["MG12", "Other Exhibit"].includes(code),
  },
  {
    category: "Forensics",
    showIfEmpty: true,
    test: ({ cmsDocType: { code } }) => ["MG22 SFR"].includes(code),
  },
  {
    category: "Unused material",
    showIfEmpty: true,
    test: ({ cmsDocType: { code } }) =>
      [
        "MG1",
        "MG6A",
        "MG6B",
        "MG6C",
        "MG6D",
        "MG6E",
        "MG20",
        "MG21",
        "MG21A",
        "PCN3",
        "MG11(R)",
      ].includes(code),
  },
  {
    category: "Defendant",
    showIfEmpty: true,
    test: ({ cmsDocType: { code } }) =>
      [
        "MG15(ROTI)",
        "MG15(SDN)",
        "MG15(ROVI)",
        "MG15(CNOI)",
        "MG16",
        "MG16(DBCI)",
        "MG16(DDOI)",
        "PE1",
        "PE2",
        "DREP",
        "PCN1",
        "PCN2",
      ].includes(code),
  },
  {
    category: "Court preparation",
    showIfEmpty: true,
    test: ({ cmsDocType: { code } }) =>
      ["MG2", "MG4B", "MG7", "MG8", "MG10", "MG13", "MG19"].includes(code),
  },
  {
    category: "Communications",
    showIfEmpty: true,
    test: ({ cmsDocType: { code } }) => ["Other Comm (In)"].includes(code),
  },
  // have unknown last so it can scoop up any unmatched documents
  {
    category: "Uncategorised",
    showIfEmpty: false,
    test: () => true,
  },
];

export const categoryNamesInPresentationOrder = documentCategoryDefinitions.map(
  ({ category }) => category
);

export const getCategory = (item: CaseDocument) =>
  documentCategoryDefinitions.find(({ test }) => test(item))!.category;

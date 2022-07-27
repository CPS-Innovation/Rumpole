import { MappedCaseDocument } from "./MappedCaseDocument";

export type CaseDocumentViewModel = MappedCaseDocument & {
  url: string | undefined;
  tabSafeId: string;
} & (
    | { mode: "read" }
    | {
        mode: "search";
        searchTerm: string;
        occurrencesInDocumentCount: number;
        pageOccurrences: { pageIndex: number; boundingBoxes: number[][] }[];
      }
  );

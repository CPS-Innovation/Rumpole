import { IPdfHighlight } from "./IPdfHighlight";
import { MappedCaseDocument } from "./MappedCaseDocument";

export type CaseDocumentViewModel = MappedCaseDocument & {
  url: string | undefined;
  tabSafeId: string;
  redactionHighlights: IPdfHighlight[];
} & (
    | { mode: "read" }
    | {
        mode: "search";
        searchTerm: string;
        occurrencesInDocumentCount: number;
        searchHighlights: IPdfHighlight[];
      }
  );

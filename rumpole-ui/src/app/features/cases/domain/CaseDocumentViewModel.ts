import { IPdfHighlight } from "./IPdfHighlight";
import { MappedCaseDocument } from "./MappedCaseDocument";

export type CaseDocumentViewModel = MappedCaseDocument & {
  url: string | undefined;
  tabSafeId: string;
  redactionHighlights: IPdfHighlight[];
  lockedState: "not-yet-locked" | "locking" | "locked" | "locked-by-other-user";
} & (
    | { mode: "read" }
    | {
        mode: "search";
        searchTerm: string;
        occurrencesInDocumentCount: number;
        searchHighlights: IPdfHighlight[];
      }
  );

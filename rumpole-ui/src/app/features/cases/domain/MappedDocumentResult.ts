import { MappedCaseDocument } from "./MappedCaseDocument";

export type MappedDocumentResult = MappedCaseDocument & {
  occurrencesInDocumentCount: number;
  occurrences: {
    id: string;
    pageIndex: number;
    contextTextChunks: { text: string; isHighlighted?: boolean }[];
    occurrencesInLine: number[][];
  }[];
  isVisible: boolean;
};

export type ApiTextSearchResult = {
  id: string;
  caseId: number;
  documentId: string;
  pageIndex: number;
  lineIndex: number;
  pageHeight: number;
  pageWidth: number;
  text: string;
  words: {
    boundingBox: number[] | null;
    text: string;
    confidence: number;
  }[];
};

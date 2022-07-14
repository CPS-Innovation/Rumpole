export type ApiTextSearchResult = {
  id: string;
  caseId: number;
  documentId: string;
  pageIndex: number;
  lineIndex: number;
  boundingBox: number[];
  text: string;
  words: {
    boundingBox: number[];
    text: string;
    confidence: number;
  }[];
};

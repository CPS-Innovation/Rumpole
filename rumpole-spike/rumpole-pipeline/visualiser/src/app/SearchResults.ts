export type SearchResults = {
  value: {
    documentId: number;
    pageIndex: number;
    lineIndex: number;
    text: string;
    boundingBox: number[];
  }[];
};

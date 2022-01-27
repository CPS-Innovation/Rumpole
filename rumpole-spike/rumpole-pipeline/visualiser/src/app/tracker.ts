export type Tracker = {
  isIndexed: boolean;
  transactionId: string;
  documents: {
    documentId: string;
    pdfUrl: string;
    pngDetails: {
      url?: string;
      dimensions?: {
        height: number;
        width: number;
      };
    }[];
  }[];
};

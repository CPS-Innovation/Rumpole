export type CaseDocument = {
  documentId: number;
  fileName: string;
  createdDate: string;
  cmsDocType: {
    id: number;
    code: string;
    name: string;
  };
};

export type CaseDocument = {
  documentId: string;
  fileName: string;
  createdDate: string;
  cmsDocType: {
    id: number;
    code: string;
    name: string;
  };
};

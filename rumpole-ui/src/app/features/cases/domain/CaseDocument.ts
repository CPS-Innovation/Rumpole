export type CaseDocument = {
  documentId: string;
  fileName: string;
  isoDate: string;
  cmsDocType: {
    code: string;
    name: string;
  };
};

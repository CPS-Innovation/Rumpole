import { CaseDocument } from "../../app/features/cases/domain/CaseDocument";
import { DocumentsDataSource } from "./types/DocumentsDataSource";

const dataSource: DocumentsDataSource = (id: string) => ({
  caseDocuments: documents,
});

export default dataSource;

const documents: CaseDocument[] = [
  {
    documentId: "d1",
    fileName: "MCLOVEMG3",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d2",
    fileName: "CM01",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d3",
    fileName: "MG05MCLOVE",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d4",
    fileName: "MG06_3June",
    createdDate: "2020-06-03",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d5",
    fileName: "MG06_10june",
    createdDate: "2020-06-10",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
];

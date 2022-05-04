import { CaseDocument } from "../../app/features/cases/domain/CaseDocument";
import { DocumentsDataSource } from "./types/DocumentsDataSource";

const dataSource: DocumentsDataSource = (id: string) => ({
  caseDocuments: documents,
});

export default dataSource;

const documents: CaseDocument[] = [
  {
    documentId: "1",
    fileName: "MCLOVE MG3",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "2",
    fileName: "MCLOVE CM01",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "3",
    fileName: "MG05 MCLOVE",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "4",
    fileName: "MG06 3 June",
    createdDate: "2020-06-03",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "5",
    fileName: "MG06 10 june",
    createdDate: "2020-06-10",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
];

import { CaseDocument } from "../../app/features/cases/domain/CaseDocument";
import { DocumentsDataSource } from "./types/DocumentsDataSource";

const dataSource: DocumentsDataSource = (id: string) => ({
  caseDocuments: documents,
});

export default dataSource;

const documents: CaseDocument[] = [
  {
    documentId: "d1",
    fileName: "MCLOVEMG3  very long .docx",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d2",
    fileName: "CM01  very long .docx",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d3",
    fileName: "MG05MCLOVE very long .docx",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d4",
    fileName: "MG06_3June  very long .docx",
    createdDate: "2020-06-03",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d5",
    fileName: "MG06_10june  very long .docx",
    createdDate: "2020-06-10",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d6",
    fileName: "MCLOVEMG3  very long .docx",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d7",
    fileName: "CM01  very long .docx",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d8",
    fileName: "MG05MCLOVE very long .docx",
    createdDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d9",
    fileName: "MG06_3June  very long .docx",
    createdDate: "2020-06-03",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d10",
    fileName: "MG06_10june  very long .docx",
    createdDate: "2020-06-10",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
];

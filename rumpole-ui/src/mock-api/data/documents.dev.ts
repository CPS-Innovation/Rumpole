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
      id: 3,
      code: "MG3",
      name: "MG3 File",
    },
  },
  {
    documentId: "d2",
    fileName: "CM01  very long .docx",
    createdDate: "2020-06-02",
    cmsDocType: {
      id: 11,
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d3",
    fileName: "MG05MCLOVE very long .docx",
    createdDate: "2020-06-02",
    cmsDocType: {
      id: 5,
      code: "MG5",
      name: "MG5 File",
    },
  },
  {
    documentId: "d4",
    fileName: "MG06_3June  very long .docx",
    createdDate: "2020-06-03",
    cmsDocType: {
      id: 6,
      code: "MG6",
      name: "MG6 File",
    },
  },
  {
    documentId: "d5",
    fileName: "MG06_10june  very long .docx",
    createdDate: "2020-06-10",
    cmsDocType: {
      id: 3,
      code: "MG3",
      name: "MG3 File",
    },
  },
  {
    documentId: "d6",
    fileName: "MCLOVEMG3  very long .docx",
    createdDate: "2020-06-02",
    cmsDocType: {
      id: 3,
      code: "MG3",
      name: "MG3 File",
    },
  },
  {
    documentId: "d7",
    fileName: "CM01  very long .docx",
    createdDate: "2020-06-02",
    cmsDocType: {
      id: -1,
      code: "Other Comm (In)",
      name: "Other Comm (In) File",
    },
  },
  {
    documentId: "d8",
    fileName: "MG05MCLOVE very long .docx",
    createdDate: "2020-06-02",
    cmsDocType: {
      id: 5,
      code: "MG5",
      name: "MG5 File",
    },
  },
  {
    documentId: "d9",
    fileName: "MG06_3June  very long .docx",
    createdDate: "2020-06-03",
    cmsDocType: {
      id: 6,
      code: "MG6",
      name: "MG6 File",
    },
  },
  {
    documentId: "d10",
    fileName: "MG06_10june  very long .docx",
    createdDate: "2020-06-10",
    cmsDocType: {
      id: 6,
      code: "MG6",
      name: "MG6 File",
    },
  },
];

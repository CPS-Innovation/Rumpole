import { CaseDocument } from "../../app/features/cases/domain/CaseDocument";
import { DocumentsDataSource } from "./types/DocumentsDataSource";

const dataSource: DocumentsDataSource = (id: string) => documents;

export default dataSource;

export const documents: CaseDocument[] = [
  {
    documentId: "1",
    fileName: "MCLOVE MG3",
    isoDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "2",
    fileName: "MCLOVE CM01",
    isoDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "3",
    fileName: "MG05 MCLOVE",
    isoDate: "2020-06-02",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "4",
    fileName: "MG06 3 June",
    isoDate: "2020-06-03",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "5",
    fileName: "MG06 10 june",
    isoDate: "2020-06-10",
    cmsDocType: {
      code: "MG11",
      name: "MG11 File",
    },
  },
];

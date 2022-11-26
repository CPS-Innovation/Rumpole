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
    createdDate: "2020-06-01",
    cmsDocType: {
      id: 1,
      code: "MG11",
      name: "MG11 File",
    },
  },
  {
    documentId: "d2",
    fileName: "CM01",
    createdDate: "2020-06-02",
    cmsDocType: {
      id: 2,
      code: "MG12",
      name: "MG12 File",
    },
  },
  {
    documentId: "d3",
    fileName: "MG05MCLOVE",
    createdDate: "2020-06-03",
    cmsDocType: {
      id: 3,
      code: "MG13",
      name: "MG13 File",
    },
  },
  {
    documentId: "d4",
    fileName: "MG06_3June",
    createdDate: "2020-06-04",
    cmsDocType: {
      id: 4,
      code: "MG14",
      name: "MG14 File",
    },
  },
  {
    documentId: "d5",
    fileName: "MG06_10june",
    createdDate: "2020-06-10",
    cmsDocType: {
      id: 5,
      code: "MG15",
      name: "MG15 File",
    },
  },
];

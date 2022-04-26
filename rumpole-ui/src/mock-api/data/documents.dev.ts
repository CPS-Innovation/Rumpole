import { CaseDocument } from "../../app/features/cases/domain/CaseDocument";
import { DocumentsDataSource } from "./types/DocumentsDataSource";

const dataSource: DocumentsDataSource = (id: string) => documents;

export default dataSource;

export const documents: CaseDocument[] = [
  { id: "1", name: "MCLOVE MG3", isoDate: "2020-06-02", category: "Reviews" },
  {
    id: "2",
    name: "MCLOVE CM01",
    isoDate: "2020-06-02",
    category: "Case overview",
  },
  {
    id: "3",
    name: "MG05 MCLOVE",
    isoDate: "2020-06-02",
    category: "Case overview",
  },
  {
    id: "4",
    name: "MG06 3 June",
    isoDate: "2020-06-03",
    category: "Case overview",
  },
  {
    id: "5",
    name: "MG06 10 june",
    isoDate: "2020-06-10",
    category: "Case overview",
  },
];

import { CaseDocument } from "./CaseDocument";

export type MappedCaseDocument = CaseDocument & {
  tabSafeId: string;
  category: string;
  presentationFileName: string;
};

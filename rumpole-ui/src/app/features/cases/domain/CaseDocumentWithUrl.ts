import { MappedCaseDocument } from "./MappedCaseDocument";

export type CaseDocumentWithUrl = MappedCaseDocument & {
  url: string | undefined;
  tabSafeId: string;
};

import { CaseDocumentWithTabSafeId } from "./CaseDocumentWithTabSafeId";

export type CaseDocumentWithUrl = CaseDocumentWithTabSafeId & {
  url: string | undefined;
  tabSafeId: string;
};

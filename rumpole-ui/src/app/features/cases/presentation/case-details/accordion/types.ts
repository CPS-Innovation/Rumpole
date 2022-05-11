import { CaseDocument } from "../../../domain/CaseDocument";

export type Section = {
  sectionId: string;
  sectionLabel: string;
  docs: CaseDocument[];
};

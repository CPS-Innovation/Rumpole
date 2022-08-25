import { RedactionSavePage } from "./RedactionSavePage";

export type RedactionSaveRequest = {
  docId: number;
  redactionPages: RedactionSavePage[];
};

import { GATEWAY_BASE_URL } from "../../../config";
import { CaseSearchResult } from "../domain/CaseSearchResult";

const fullPath = (path: string) => new URL(path, GATEWAY_BASE_URL).toString();

export const searchUrn = async (urn: string) => {
  const response = await fetch(fullPath(`/cases/search/?urn=${urn}`));
  return (await response.json()) as CaseSearchResult[];
};

import { CaseSearchResult } from "../domain/CaseSearchResult";

const BASE_URL = "https://api";

export const searchUrn = async (urn: string) => {
  const response = await fetch(`${BASE_URL}/cases/search/?urn=${urn}`);
  return (await response.json()) as CaseSearchResult[];
};

import { searchCase } from "../../api/gateway-api";

export const searchCaseWhenReady = async (
  id: number,
  searchTerm: string,
  isPipelineComplete: boolean,
  isDocumentCallComplete: boolean
) => {
  return searchTerm && isPipelineComplete && isDocumentCallComplete
    ? await searchCase(id, searchTerm)
    : undefined;
};

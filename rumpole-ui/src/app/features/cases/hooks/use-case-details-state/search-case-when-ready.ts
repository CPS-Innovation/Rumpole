import { searchCase } from "../../api/gateway-api";

export const searchCaseWhenReady = async (
  id: string,
  searchTerm: string,
  isPipelineComplete: boolean,
  isDocumentCallComplete: boolean
) => {
  return searchTerm && isPipelineComplete && isDocumentCallComplete
    ? await searchCase(id, searchTerm)
    : undefined;
};

import { getAccessToken } from "../../../auth";
import { ApiError } from "../../../common/errors/ApiError";
import { GATEWAY_BASE_URL, GATEWAY_SCOPE } from "../../../config";
import { CaseDocument } from "../domain/CaseDocument";
import { CaseSearchResult } from "../domain/CaseSearchResult";

const getFullPath = (path: string) => {
  return new URL(path, GATEWAY_BASE_URL).toString();
};

const getHeaders = async () => {
  if (!GATEWAY_SCOPE) {
    // if we are dev test mode with mocked auth we don't want to try to
    //  get a token
    return undefined;
  }
  return new Headers({
    Authorization: `Bearer ${await getAccessToken([GATEWAY_SCOPE])}`,
  });
};

export const searchUrn = async (urn: string) => {
  const headers = await getHeaders();
  const path = getFullPath(`/api/case-information-by-urn/${urn}`);
  const response = await fetch(path, {
    headers,
    method: "GET",
  });

  if (!response.ok) {
    // special case: the gateway returns 404 if no results
    //  but we are happy to just return empty data
    if (response.status === 404) {
      return [];
    }
    throw new ApiError("Search URN failed", path, response);
  }

  return (await response.json()) as CaseSearchResult[];
};

export const getCaseDetails = async (caseId: string) => {
  const headers = await getHeaders();
  const path = getFullPath(`/api/case-details/${caseId}`);
  const response = await fetch(path, {
    headers,
    method: "GET",
  });

  if (!response.ok) {
    throw new ApiError("Get Case Details failed", path, response);
  }

  return (await response.json()) as CaseSearchResult;
};

export const getCaseDocuments = async (caseId: string) => {
  const headers = await getHeaders();
  const path = getFullPath(`/api/case-documents/${caseId}`);
  const response = await fetch(path, {
    headers,
    method: "GET",
  });

  if (!response.ok) {
    throw new ApiError("Get Case Documents failed", path, response);
  }

  const apiReponse: { caseDocuments: CaseDocument[] } = await response.json();

  return apiReponse.caseDocuments;
};

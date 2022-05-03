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
  const response = await fetch(
    getFullPath(`/api/case-information-by-urn/${urn}`),
    {
      headers,
      method: "GET",
    }
  );

  if (!response.ok) {
    throw new ApiError("Search URN failed", response);
  }

  return (await response.json()) as CaseSearchResult[];
};

export const getCaseDetails = async (caseId: string) => {
  const headers = await getHeaders();
  const response = await fetch(getFullPath(`/api/case-details/${caseId}`), {
    headers,
    method: "GET",
  });

  if (!response.ok) {
    throw new ApiError("Get Case Details failed", response);
  }

  return (await response.json()) as CaseSearchResult;
};

export const getCaseDocuments = async (caseId: string) => {
  const headers = await getHeaders();
  const response = await fetch(getFullPath(`/api/case-documents/${caseId}`), {
    headers,
    method: "GET",
  });

  if (!response.ok) {
    if (response.status === 404) {
      // if there are no docs then the gatewy returns 404, we don't need this to be a failure
      return [];
    }
    throw new ApiError("Get Case Documents failed", response);
  }

  const apiReponse: { caseDocuments: CaseDocument[] } = await response.json();

  return apiReponse.caseDocuments;
};

import { getAccessToken } from "../../../auth";
import { ApiError } from "../../../common/errors/ApiError";
import { GATEWAY_BASE_URL, GATEWAY_SCOPE } from "../../../config";
import { CaseDocument } from "../domain/CaseDocument";
import { CaseSearchResult } from "../domain/CaseSearchResult";
import { PipelineResults } from "../domain/PipelineResults";
import { ApiTextSearchResult } from "../domain/ApiTextSearchResult";
import { RedactionSaveRequest } from "../domain/RedactionSaveRequest";
import { RedactionSaveResponse } from "../domain/RedactionSaveResponse";

const getFullUrl = (path: string) => {
  return new URL(path, GATEWAY_BASE_URL).toString();
};

export const getCoreHeaders = async () => {
  if (!GATEWAY_SCOPE) {
    // if we are dev test mode with mocked auth we don't want to try to
    //  get a token
    return new Headers({
      Authorization: `Bearer TEST`,
    });
  }
  return new Headers({
    Authorization: `Bearer ${await getAccessToken([GATEWAY_SCOPE])}`,
  });
};

export const resolvePdfUrl = (blobName: string) =>
  getFullUrl(`/api/pdfs/${blobName}`);

export const searchUrn = async (urn: string) => {
  const headers = await getCoreHeaders();
  const path = getFullUrl(`/api/case-information-by-urn/${urn}`);
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
  const headers = await getCoreHeaders();
  const path = getFullUrl(`/api/case-details/${caseId}`);
  const response = await fetch(path, {
    headers,
    method: "GET",
  });

  if (!response.ok) {
    throw new ApiError("Get Case Details failed", path, response);
  }

  return (await response.json()) as CaseSearchResult;
};

export const getCaseDocumentsList = async (caseId: string) => {
  const headers = await getCoreHeaders();
  const path = getFullUrl(`/api/case-documents/${caseId}`);
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

export const getPdfSasUrl = async (pdfBlobName: string) => {
  const headers = await getCoreHeaders();
  const path = getFullUrl(`/api/pdf/sasUrl/${pdfBlobName}`);
  const response = await fetch(path, {
    headers,
    method: "GET",
  });

  if (!response.ok) {
    throw new ApiError("Get Pdf SasUrl failed", path, response);
  }

  return await response.text();
};

export const initiatePipeline = async (caseId: string) => {
  const headers = await getCoreHeaders();
  const path = getFullUrl(`/api/cases/${caseId}?force=true`);
  const response = await fetch(path, {
    headers,
    method: "POST",
  });

  if (!response.ok) {
    throw new ApiError("Initiate pipeline failed", path, response);
  }

  const apiReponse: { trackerUrl: string } = await response.json();

  return apiReponse.trackerUrl;
};

export const getPipelinePdfResults = async (trackerUrl: string) => {
  const headers = await getCoreHeaders();
  const response = await fetch(trackerUrl, {
    headers,
    method: "GET",
  });

  return (await response.json()) as PipelineResults;
};

export const searchCase = async (caseId: string, searchTerm: string) => {
  const headers = await getCoreHeaders();
  const path = getFullUrl(`/api/cases/${caseId}/query/${searchTerm}`);
  const response = await fetch(path, {
    headers,
    method: "GET",
  });

  if (!response.ok) {
    throw new ApiError("Search Case Text failed", path, response);
  }

  return (await response.json()) as ApiTextSearchResult[];
};

export const checkoutDocument = async (caseId: string, docId: string) => {
  const headers = await getCoreHeaders();
  const path = getFullUrl(`/api/documents/checkout/${caseId}/${docId}`);
  const response = await fetch(path, {
    headers,
    method: "PUT",
  });

  if (!response.ok) {
    throw new ApiError("Checkout document failed", path, response);
  }

  return true; // unhappy path not known yet
};

export const checkinDocument = async (caseId: string, docId: string) => {
  const headers = await getCoreHeaders();
  const path = getFullUrl(`/api/documents/checkin/${caseId}/${docId}`);
  const response = await fetch(path, {
    headers,
    method: "PUT",
  });

  if (!response.ok) {
    throw new ApiError("Checkin document failed", path, response);
  }

  return true; // unhappy path not known yet
};

export const saveRedactions = async (
  caseId: string,
  docId: string,
  fileName: string,
  redactionSaveRequest: RedactionSaveRequest
) => {
  const headers = await getCoreHeaders();
  const path = getFullUrl(
    `/api/documents/saveRedactions/${caseId}/${docId}/${fileName}`
  );

  const response = await fetch(path, {
    headers,
    method: "PUT",
    body: JSON.stringify(redactionSaveRequest),
  });

  if (!response.ok) {
    throw new ApiError("Save redactions failed", path, response);
  }

  return (await response.json()) as RedactionSaveResponse;
};

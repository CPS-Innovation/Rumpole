import { getAccessToken } from "../../../auth";
import { ApiError } from "../../../common/errors/ApiError";
import { GATEWAY_BASE_URL, GATEWAY_SCOPE } from "../../../config";
import { CaseDocument } from "../domain/CaseDocument";
import { CaseSearchResult } from "../domain/CaseSearchResult";
import { PipelineResults } from "../domain/PipelineResults";
import { ApiTextSearchResult } from "../domain/ApiTextSearchResult";
import { RedactionSaveRequest } from "../domain/RedactionSaveRequest";
import { RedactionSaveResponse } from "../domain/RedactionSaveResponse";
import { v4 as uuidv4 } from "uuid";

const CORRELATION_ID = "Correlation-Id";

const getFullUrl = (path: string) => {
  return new URL(path, GATEWAY_BASE_URL).toString();
};

const generateCorrelationId = () => uuidv4();

export const getCoreHeadersInitObject = async () => {
  return {
    [CORRELATION_ID]: generateCorrelationId(),
    Authorization: `Bearer ${
      GATEWAY_SCOPE ? await getAccessToken([GATEWAY_SCOPE]) : "TEST"
    }`,
    "Upstream-Token": "not-implemented-yet", //todo
  };
};

const getCoreHeaders = async (init?: HeadersInit | undefined) => {
  return new Headers({
    ...(await getCoreHeadersInitObject()),
    // allow init to override any headers created here
    ...init,
  });
};

export const resolvePdfUrl = (blobName: string) =>
  getFullUrl(`/api/pdfs/${blobName}`);

export const searchUrn = async (urn: string) => {
  const headers = await getCoreHeaders();
  const path = getFullUrl(`/api/urns/${urn}`);
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

  const { trackerUrl }: { trackerUrl: string } = await response.json();

  return { trackerUrl, correlationId: headers.get(CORRELATION_ID)! };
};

export const getPipelinePdfResults = async (
  trackerUrl: string,
  correlationId: string
) => {
  const headers = await getCoreHeaders({
    [CORRELATION_ID]: correlationId,
  });

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

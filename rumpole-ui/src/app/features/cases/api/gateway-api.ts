import { ApiError } from "../../../common/errors/ApiError";

import { CaseDocument } from "../domain/CaseDocument";
import { CaseSearchResult } from "../domain/CaseSearchResult";
import { PipelineResults } from "../domain/PipelineResults";
import { ApiTextSearchResult } from "../domain/ApiTextSearchResult";
import { RedactionSaveRequest } from "../domain/RedactionSaveRequest";
import { RedactionSaveResponse } from "../domain/RedactionSaveResponse";
import * as HEADERS from "./header-factory";
import {
  buildFullUrl as buildEncodedUrl,
  fullUrl as buildUnencodedUrl,
} from "./url-helpers";

const buildHeaders = async (
  ...args: (
    | Record<string, string>
    | (() => Record<string, string>)
    | (() => Promise<Record<string, string>>)
  )[]
) => {
  const headers = [] as Record<string, string>[];
  for (const arg of args) {
    const header = typeof arg === "function" ? await arg() : arg; // unwrap if a promise. otherwise all good
    headers.push(header);
  }
  return new Headers(...headers);
};

export const resolvePdfUrl = (blobNameUrlFragment: string) =>
  buildUnencodedUrl(`/api/pdfs/${blobNameUrlFragment}`);

export const searchUrn = async (urn: string) => {
  const url = buildEncodedUrl({ urn }, ({ urn }) => `/api/urns/${urn}/cases`);
  const response = await fetch(url, {
    headers: await buildHeaders(
      HEADERS.correlationId,
      HEADERS.auth,
      HEADERS.upstreamHeader
    ),
  });

  if (!response.ok) {
    // special case: the gateway returns 404 if no results
    //  but we are happy to just return empty data
    if (response.status === 404) {
      return [];
    }
    throw new ApiError("Search URN failed", url, response);
  }

  return (await response.json()) as CaseSearchResult[];
};

export const getCaseDetails = async (urn: string, caseId: number) => {
  const url = buildEncodedUrl(
    { urn, caseId },
    ({ urn, caseId }) => `/api/urns/${urn}/cases/${caseId}`
  );

  const response = await fetch(url, {
    headers: await buildHeaders(
      HEADERS.correlationId,
      HEADERS.auth,
      HEADERS.upstreamHeader
    ),
  });

  if (!response.ok) {
    throw new ApiError("Get Case Details failed", url, response);
  }

  return (await response.json()) as CaseSearchResult;
};

export const getCaseDocumentsList = async (urn: string, caseId: number) => {
  const url = buildEncodedUrl(
    { urn, caseId },
    ({ urn, caseId }) => `/api/urns/${urn}/cases/${caseId}/documents`
  );
  const response = await fetch(url, {
    headers: await buildHeaders(
      HEADERS.correlationId,
      HEADERS.auth,
      HEADERS.upstreamHeader
    ),
  });

  if (!response.ok) {
    throw new ApiError("Get Case Documents failed", url, response);
  }

  const apiReponse: CaseDocument[] = await response.json();

  return apiReponse;
};

export const getPdfSasUrl = async (pdfBlobName: string) => {
  const url = buildUnencodedUrl(`/api/pdf/sasUrl/${pdfBlobName}`);
  const response = await fetch(url, {
    headers: await buildHeaders(HEADERS.correlationId, HEADERS.auth),
  });

  if (!response.ok) {
    throw new ApiError("Get Pdf SasUrl failed", url, response);
  }

  return await response.text();
};

export const initiatePipeline = async (urn: string, caseId: number) => {
  const path = buildEncodedUrl(
    { urn, caseId },
    ({ urn, caseId }) => `/api/cases/${urn}/${caseId}?force=true`
  );

  const correlationIdHeader = HEADERS.correlationId();
  const response = await fetch(path, {
    headers: await buildHeaders(
      correlationIdHeader,
      HEADERS.auth,
      HEADERS.upstreamHeader
    ),
    method: "POST",
  });

  if (!response.ok) {
    throw new ApiError("Initiate pipeline failed", path, response);
  }

  const { trackerUrl }: { trackerUrl: string } = await response.json();

  return { trackerUrl, correlationId: Object.values(correlationIdHeader)[0] };
};

export const getPipelinePdfResults = async (
  trackerUrl: string,
  existingCorrelationId: string
) => {
  const headers = await buildHeaders(
    HEADERS.correlationId(existingCorrelationId),
    HEADERS.auth
  );

  const response = await fetch(trackerUrl, {
    headers,
  });

  return (await response.json()) as PipelineResults;
};

export const searchCase = async (caseId: number, searchTerm: string) => {
  const path = buildEncodedUrl(
    { caseId, searchTerm },
    ({ caseId, searchTerm }) => `/api/cases/${caseId}/query/${searchTerm}`
  );
  const response = await fetch(path, {
    headers: await buildHeaders(
      HEADERS.correlationId,
      HEADERS.auth,
      HEADERS.upstreamHeader
    ),
  });

  if (!response.ok) {
    throw new ApiError("Search Case Text failed", path, response);
  }

  return (await response.json()) as ApiTextSearchResult[];
};

export const checkoutDocument = async (caseId: number, docId: number) => {
  const url = buildEncodedUrl(
    { caseId, docId },
    ({ caseId, docId }) => `/api/documents/checkout/${caseId}/${docId}`
  );

  const response = await fetch(url, {
    headers: await buildHeaders(
      HEADERS.correlationId,
      HEADERS.auth,
      HEADERS.upstreamHeader
    ),
    method: "PUT",
  });

  if (!response.ok) {
    throw new ApiError("Checkout document failed", url, response);
  }

  return true; // unhappy path not known yet
};

export const checkinDocument = async (caseId: number, docId: number) => {
  const url = buildEncodedUrl(
    { caseId, docId },
    ({ caseId, docId }) => `/api/documents/checkout/${caseId}/${docId}`
  );

  const response = await fetch(url, {
    headers: await buildHeaders(
      HEADERS.correlationId,
      HEADERS.auth,
      HEADERS.upstreamHeader
    ),
    method: "PUT",
  });

  if (!response.ok) {
    throw new ApiError("Checkin document failed", url, response);
  }

  return true; // unhappy path not known yet
};

export const saveRedactions = async (
  caseId: number,
  docId: number,
  fileName: string,
  redactionSaveRequest: RedactionSaveRequest
) => {
  const url = buildEncodedUrl(
    { caseId, docId, fileName },
    ({ caseId, docId, fileName }) =>
      `/api/documents/saveRedactions/${caseId}/${docId}/${fileName}`
  );

  const response = await fetch(url, {
    headers: await buildHeaders(
      HEADERS.correlationId,
      HEADERS.auth,
      HEADERS.upstreamHeader
    ),
    method: "PUT",
    body: JSON.stringify(redactionSaveRequest),
  });

  if (!response.ok) {
    throw new ApiError("Save redactions failed", url, response);
  }

  return (await response.json()) as RedactionSaveResponse;
};

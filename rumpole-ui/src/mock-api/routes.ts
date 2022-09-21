export const CASE_SEARCH_ROUTE = "api/case-information-by-urn/*";
export const CASE_ROUTE = "api/case-details/:id";
export const DOCUMENTS_ROUTE = "api/case-documents/:id";
export const INITIATE_PIPELINE_ROUTE = "api/cases/:caseId";
export const TRACKER_ROUTE = "api/cases/:caseId/tracker";
export const FILE_ROUTE = "api/pdfs/:blobName";
export const TEXT_SEARCH_ROUTE = "api/cases/:caseId/query/:query";
export const DOCUMENT_CHECKOUT_ROUTE =
  "api/documents/checkout/:caseId/:documentId";
export const DOCUMENT_CHECKIN_ROUTE =
  "api/documents/checkin/:caseId/:documentId";
export const GET_SAS_URL_ROUTE = "api/pdf/sas-url/:blobName";
export const SAS_URL_ROUTE = "api/some-complicated-sas-url/:blobName";

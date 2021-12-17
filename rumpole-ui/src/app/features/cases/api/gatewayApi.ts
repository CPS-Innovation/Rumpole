import { getAccessToken } from "../../../auth";
import { GATEWAY_BASE_URL } from "../../../config";
import { CaseSearchResult } from "../domain/CaseSearchResult";

const getFullPath = (path: string) => {
  return new URL(path, GATEWAY_BASE_URL).toString();
};

const getHeaders = async () =>
  new Headers({
    Authorization: `Bearer ${await getAccessToken([
      "https://CPSGOVUK.onmicrosoft.com/fa-rumpole-dev-gateway/user_impersonation",
    ])}`,
  });

export const searchUrn = async (urn: string) => {
  const headers = await getHeaders();
  const response = await fetch(getFullPath(`/cases/search/?urn=${urn}`), {
    headers,
    method: "GET",
  });
  return (await response.json()) as CaseSearchResult[];
};

export const handshake = async (caseId: number) => {
  const headers = await getHeaders();
  const response = await fetch(getFullPath(`api/case-details/${caseId}`), {
    headers,
    method: "GET",
  });
  return (await response.json()) as { caseType: string };
};

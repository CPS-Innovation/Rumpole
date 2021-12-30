import { getAccessToken } from "../../../auth";
import { ApiError } from "../../../common/errors/ApiError";
import { GATEWAY_BASE_URL, GATEWAY_SCOPE } from "../../../config";
import { CaseSearchResult } from "../domain/CaseSearchResult";

const getFullPath = (path: string) => {
  return new URL(path, GATEWAY_BASE_URL).toString();
};

const getHeaders = async () =>
  new Headers({
    Authorization: `Bearer ${await getAccessToken([GATEWAY_SCOPE])}`,
  });

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

import { getAccessToken } from "../../../auth";
import { GATEWAY_BASE_URL } from "../../../config";
import { CaseSearchResult } from "../domain/CaseSearchResult";

const getHeaders = async () =>
  new Headers({
    Authorization: `Bearer ${await getAccessToken(["User.Read"])}`,
  });

export const searchUrn = async (urn: string) => {
  const headers = await getHeaders();
  const response = await fetch(GATEWAY_BASE_URL, {
    headers,
    method: "POST",
    body: JSON.stringify({
      query: `{
        cases(urn: "${urn}") {
          id
          uniqueReferenceNumber
          area {
            name
            code
          }
          appealType
          caseStatus
          {
            code
            description
          }
          caseType
          documents {
            id
            type{
              code
              name
            }
          }
          leadDefendant {
            firstNames
            surname
            organisationName
          }
          offences{
            earlyDate
            lateDate
            listOrder
            code
            shortDescription
            longDescription
          }
        }
      }`,
    }),
  });
  return (await response.json()) as CaseSearchResult[];
};

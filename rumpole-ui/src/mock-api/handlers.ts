import { rest } from "msw";
import devSearchDataSource from "./data/searchResults.dev";
import cypressSearchDataSource from "./data/searchResults.cypress";
import devCaseDetailsDataSource from "./data/caseDetails.dev";
import cypressDetailsDataSource from "./data/caseDetails.cypress";

import devDocumentsDataSource from "./data/documents.dev";

import { CaseSearchResult } from "../app/features/cases/domain/CaseSearchResult";
import { SearchDataSource } from "./data/types/SearchDataSource";
import {
  CaseDetailsDataSource,
  lastRequestedUrnCache,
} from "./data/types/CaseDetailsDataSource";
import { DocumentsDataSource } from "./data/types/DocumentsDataSource";

import { CaseDetails } from "../app/features/cases/domain/CaseDetails";
import { CaseDocument } from "../app/features/cases/domain/CaseDocument";

const searchDataSources: { [key: string]: SearchDataSource } = {
  dev: devSearchDataSource,
  cypress: cypressSearchDataSource,
};

const caseDetailsDataSources: { [key: string]: CaseDetailsDataSource } = {
  dev: devCaseDetailsDataSource,
  cypress: cypressDetailsDataSource,
};

const documentsDataSources: { [key: string]: DocumentsDataSource } = {
  dev: devDocumentsDataSource,
};

const activeSourceNames = (process.env.REACT_APP_MOCK_API_SOURCES ?? "").split(
  ","
);

const delayFactor = Number(process.env.REACT_APP_MOCK_API_MAX_DELAY || 0);
console.log(delayFactor);
const apiPath = (path: string) =>
  new URL(path, process.env.REACT_APP_GATEWAY_BASE_URL).toString();

export const handlers = [
  rest.get(apiPath("api/case-information-by-urn/*"), (req, res, ctx) => {
    const urn = req.url.pathname.split("/").pop()!;
    lastRequestedUrnCache.urn = urn;

    let results: CaseSearchResult[] = [];

    for (const sourceName of activeSourceNames) {
      if (searchDataSources[sourceName]) {
        const data = searchDataSources[sourceName](urn);
        if (data.length) {
          results = data;
          break;
        }
      }
    }

    return res(ctx.delay(Math.random() * delayFactor), ctx.json(results));
  }),

  rest.get(apiPath("api/case-details/:id"), (req, res, ctx) => {
    const { id } = req.params;

    let result: CaseDetails | undefined = undefined;

    for (const source of activeSourceNames) {
      if (caseDetailsDataSources[source]) {
        const data = caseDetailsDataSources[source](id);
        if (data) {
          result = {
            ...data,
            uniqueReferenceNumber: lastRequestedUrnCache.urn || "99ZZ9999999",
          };
          break;
        }
      }
    }

    return res(ctx.delay(Math.random() * delayFactor), ctx.json(result));
  }),

  rest.get(apiPath("api/case-details/:id/documents"), (req, res, ctx) => {
    const { id } = req.params;

    let result: CaseDocument[] = [];

    for (const source of activeSourceNames) {
      if (documentsDataSources[source]) {
        const data = documentsDataSources[source](id);
        if (data) {
          result = data;
          break;
        }
      }
    }

    return res(ctx.delay(Math.random() * delayFactor), ctx.json(result));
  }),
];

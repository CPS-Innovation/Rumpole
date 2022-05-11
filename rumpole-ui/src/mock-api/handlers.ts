import { rest, RestContext } from "msw";
import devSearchDataSource from "./data/searchResults.dev";
import cypressSearchDataSource from "./data/searchResults.cypress";
import devCaseDetailsDataSource from "./data/caseDetails.dev";
import cypressDetailsDataSource from "./data/caseDetails.cypress";
import devDocumentsDataSource from "./data/documents.dev";
import cypressDocumentsDataSource from "./data/documents.cypress";
import { SearchDataSource } from "./data/types/SearchDataSource";
import {
  CaseDetailsDataSource,
  lastRequestedUrnCache,
} from "./data/types/CaseDetailsDataSource";
import { DocumentsDataSource } from "./data/types/DocumentsDataSource";
import { MockApiConfig } from "./MockApiConfig";

// eslint-disable-next-line import/no-webpack-loader-syntax
import fileStrings from "./data/files/file-strings.json";

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
  cypress: cypressDocumentsDataSource,
};

export const setupHandlers = ({
  sourceName,
  maxDelay,
  baseUrl,
}: MockApiConfig) => {
  const makeApiPath = (path: string) => new URL(path, baseUrl).toString();

  const delay = (ctx: RestContext) => ctx.delay(Math.random() * maxDelay);

  return [
    rest.get(makeApiPath("api/case-information-by-urn/*"), (req, res, ctx) => {
      const urn = req.url.pathname.split("/").pop()!;
      lastRequestedUrnCache.urn = urn;

      const results = searchDataSources[sourceName](urn);

      return res(delay(ctx), ctx.json(results));
    }),

    rest.get(makeApiPath("api/case-details/:id"), (req, res, ctx) => {
      const { id } = req.params;

      const result = {
        ...caseDetailsDataSources[sourceName](id),
        uniqueReferenceNumber: lastRequestedUrnCache.urn || "99ZZ9999999",
      };

      return res(delay(ctx), ctx.json(result));
    }),

    rest.get(makeApiPath("api/case-documents/:id"), (req, res, ctx) => {
      const { id } = req.params;

      const result = documentsDataSources[sourceName](id);

      return res(delay(ctx), ctx.json(result));
    }),

    rest.get(
      makeApiPath("api/documents/:documentId/:fileName"),
      (req, res, ctx) => {
        const { fileName } = req.params;

        const fileBase64 = (fileStrings as { [key: string]: string })[fileName];

        return res(delay(ctx), ctx.body(_base64ToArrayBuffer(fileBase64)));
      }
    ),
  ];
};

function _base64ToArrayBuffer(base64: string) {
  var binary_string = window.atob(base64);
  var len = binary_string.length;
  var bytes = new Uint8Array(len);
  for (var i = 0; i < len; i++) {
    bytes[i] = binary_string.charCodeAt(i);
  }
  return bytes.buffer;
}

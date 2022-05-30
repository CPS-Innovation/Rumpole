import { rest, RestContext } from "msw";

import devSearchDataSource from "./data/searchResults.dev";
import cypressSearchDataSource from "./data/searchResults.cypress";
import devCaseDetailsDataSource from "./data/caseDetails.dev";
import cypressDetailsDataSource from "./data/caseDetails.cypress";
import devDocumentsDataSource from "./data/documents.dev";
import cypressDocumentsDataSource from "./data/documents.cypress";
import devpipelinePdfResultsDataSource from "./data/pipelinePdfResults.dev";
import cypresspipelinePdfResultsDataSource from "./data/pipelinePdfResults.cypress";

import { SearchDataSource } from "./data/types/SearchDataSource";
import {
  CaseDetailsDataSource,
  lastRequestedUrnCache,
} from "./data/types/CaseDetailsDataSource";
import { DocumentsDataSource } from "./data/types/DocumentsDataSource";
import { PipelinePdfResultsDataSource } from "./data/types/PipelinePdfResultsDataSource";

import { MockApiConfig } from "./MockApiConfig";

// eslint-disable-next-line import/no-webpack-loader-syntax
import pdfStrings from "./data/pdfs/pdf-strings.json";

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

const pipelinePdfResultsDataSources: {
  [key: string]: PipelinePdfResultsDataSource;
} = {
  dev: devpipelinePdfResultsDataSource,
  cypress: cypresspipelinePdfResultsDataSource,
};

export const setupHandlers = ({
  sourceName,
  maxDelayMs,
  baseUrl,
}: MockApiConfig) => {
  // make sure we are reading a number not string from config
  //  also msw will not accept a delay of 0, so if 0 is passed then just set to 1ms
  const sanitisedMaxDelay = Number(maxDelayMs) || 1;

  const makeApiPath = (path: string) => new URL(path, baseUrl).toString();

  const delay = (ctx: RestContext) =>
    ctx.delay(Math.random() * sanitisedMaxDelay);

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
        throw new Error("Not implemented");
      }
    ),

    rest.post(makeApiPath("api/cases/:caseId"), (req, res, ctx) => {
      const { caseId } = req.params;
      return res(
        delay(ctx),
        ctx.json({ trackerUrl: makeApiPath(`api/cases/${caseId}/tracker`) })
      );
    }),

    rest.get(makeApiPath("api/cases/:caseId/tracker"), (req, res, ctx) => {
      const result = pipelinePdfResultsDataSources[sourceName]();

      // always maxDelay as we want this to be slow to illustrate async nature of tracker/polling
      return res(ctx.delay(sanitisedMaxDelay), ctx.json(result));
    }),

    rest.get(makeApiPath("api/pdfs/:blobName"), (req, res, ctx) => {
      const { blobName } = req.params;

      const fileBase64 = (pdfStrings as { [key: string]: string })[blobName];

      return res(delay(ctx), ctx.body(_base64ToArrayBuffer(fileBase64)));
    }),
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

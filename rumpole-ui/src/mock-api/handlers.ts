import { rest } from "msw";
import devDataSource from "./data/searchResults.dev";
import cypressDataSource from "./data/searchResults.cypress";
import { CaseSearchResult } from "../app/features/cases/domain/CaseSearchResult";
import { SearchDataSource } from "./data/types/SearchDataSource";

const dataSources: { [key: string]: SearchDataSource } = {
  dev: devDataSource,
  cypress: cypressDataSource,
};

const sources = (process.env.REACT_APP_MOCK_API_SOURCES ?? "").split(",");

const apiPath = (path: string) =>
  new URL(path, process.env.REACT_APP_GATEWAY_BASE_URL).toString();

export const handlers = [
  rest.get(apiPath("api/case-information-by-urn/*"), (req, res, ctx) => {
    const urn = req.url.pathname.split("/").pop()!;

    let results: CaseSearchResult[] = [];

    for (const source of sources) {
      if (dataSources[source]) {
        const data = dataSources[source](urn);
        if (data.length) {
          results = data;
          break;
        }
      }
    }

    const delayFactor = Number(process.env.REACT_APP_MOCK_API_MAX_DELAY || 0);
    return res(ctx.delay(Math.random() * delayFactor), ctx.json(results));
  }),
];

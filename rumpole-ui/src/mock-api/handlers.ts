import { rest } from "msw";
import demoDataSource from "./data/searchResults.demo";
import devDataSource from "./data/searchResults.dev";
import integrationDataSource from "./data/searchResults.integration";
import cypressDataSource from "./data/searchResults.cypress";
import { CaseSearchResult } from "../app/features/cases/domain/CaseSearchResult";
import { SearchDataSource } from "./data/types/SearchDataSource";

const dataSources: { [key: string]: SearchDataSource } = {
  demo: demoDataSource,
  dev: devDataSource,
  integration: integrationDataSource,
  cypress: cypressDataSource,
};

const sources = (process.env.REACT_APP_MOCK_API_SOURCES ?? "").split(",");

const apiPath = (path: string) =>
  new URL(path, process.env.REACT_APP_GATEWAY_BASE_URL).toString();

export const handlers = [
  rest.get(apiPath("cases/search/*"), (req, res, ctx) => {
    const urn = req.url.searchParams.get("urn")!;

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

    return res(ctx.delay(Math.random() * 2500), ctx.json(results));
  }),
];

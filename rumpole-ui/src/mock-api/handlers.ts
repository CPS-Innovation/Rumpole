import { rest } from "msw";

import { searchResults } from "./data/searchResults";

const api = (path: string) =>
  new URL(path, process.env.REACT_APP_GATEWAY_BASE_URL).toString();

export const handlers = [
  rest.get(api("cases/search/*"), (req, res, ctx) => {
    const urn = req.url.searchParams.get("urn");
    const lastDigit = Number(urn?.split("").pop());

    return res(
      ctx.delay(Math.random() * 2000),
      ctx.json(lastDigit ? [...searchResults].slice(-1 * lastDigit) : [])
    );
  }),
];

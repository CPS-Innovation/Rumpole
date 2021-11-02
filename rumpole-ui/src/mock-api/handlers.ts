import { rest } from "msw";

import { searchResults } from "./data/searchResults";

const api = (path: string) => new URL(path, "https://api").toString();

export const handlers = [
  rest.get(api("cases/search/*"), (req, res, ctx) => {
    const urn = req.url.searchParams.get("urn");
    const lastDigit = Number(urn?.split("").pop());

    return res(
      ctx.status(200),
      ctx.json([...searchResults].slice(-1 * lastDigit))
    );
  }),
];

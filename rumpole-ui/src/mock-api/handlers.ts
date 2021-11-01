import { rest } from "msw";

const api = (path: string) =>
  new URL(path, "https://jsonplaceholder.typicode.com").toString();

export const handlers = [
  rest.get(api("/users"), (req, res, ctx) => {
    return res(
      ctx.status(200),
      ctx.json([
        {
          caseId: 1,
          name: "foo",
        },
        {
          caseId: 7,
          name: "bar",
        },
      ])
    );
  }),
];

// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })
import "@testing-library/cypress/add-commands";
import { rest as mswRest } from "msw";
declare global {
  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace Cypress {
    interface Chainable<Subject> {
      visitPageAndbreakApiRoute(
        pageRoute: string,
        apiRoute: string,
        httpStatusCode: number
      ): Chainable<AUTWindow>;
    }
  }
}
const apiPath = (path: string) =>
  new URL(path, Cypress.env("REACT_APP_GATEWAY_BASE_URL")).toString();

Cypress.Commands.add(
  "visitPageAndbreakApiRoute",
  (pageRoute, apiRoute, httpStatusCode) => {
    return cy
      .visit(pageRoute) // ensure we are loaded and `window` is set
      .window()
      .then((/*window*/) => {
        // the `window` object passed into the function does not have `msw`
        //  attached to it - so god knows what's happening.  The ambient
        //  `window` does have msw, so just use that.
        const msw = (window as any).msw;

        msw.worker.use(
          (msw.rest as typeof mswRest).get(
            apiPath(apiRoute),
            (req, res, ctx) => {
              return res.once(ctx.status(httpStatusCode));
            }
          )
        );
      });
  }
);

export {};

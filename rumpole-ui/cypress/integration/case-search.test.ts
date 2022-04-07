describe("searching for cases", () => {
  it("can not accept an invalid URN and return an appropriate validation message to the user", () => {
    cy.visit("/case-search");
    cy.findAllByTestId("input-search-urn-error").should("not.exist");

    cy.findByTestId("input-search-urn").type("XXX");
    cy.findByTestId("button-search").click();

    cy.findAllByTestId("input-search-urn-error").should("exist");
  });

  it("can accept a valid URN and redirect to the results page with the expected urn query parameter", () => {
    cy.visit("/case-search");

    cy.findByTestId("input-search-urn").type("12AB1111111");

    cy.findByTestId("button-search").click();

    cy.location("pathname").should("eq", "/case-search-results");
    cy.location("search").should("eq", "?urn=12AB1111111");
  });

  it("can trigger search by hitting enter in the search field", () => {
    cy.visit("/case-search");

    cy.findByTestId("input-search-urn").type("12AB1111111{enter}");

    cy.location("pathname").should("eq", "/case-search-results");
  });
});

export {};

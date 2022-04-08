describe("search results", () => {
  it.only("displays search result and summarises returned count and URN", () => {
    cy.visit("/case-search-results?urn=12AB1111111");

    cy.findByTestId("link-12AB1111111");
    cy.findByTestId("txt-result-count").contains("1");
    cy.findByTestId("txt-result-urn").contains("12AB1111111");

    cy.visit("/case-search-results?urn=12AB2222222");

    cy.findByTestId("link-12AB2222222/1");
    cy.findByTestId("link-12AB2222222/2");
    cy.findByTestId("txt-result-count").contains("2");
    cy.findByTestId("txt-result-urn").contains("12AB2222222");
  });
});

export {};

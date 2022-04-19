describe("case details page", () => {
  describe("case page navigation", () => {
    it("can navigate back from case page, having not previously visited results page, and land on search page", () => {
      cy.visit("/case-details/13401");

      cy.findAllByTestId("link-back-link").click();
      cy.location("pathname").should("eq", "/case-search");
    });

    it("can navigate back from case page, having previously visited results page, and land on results page", () => {
      cy.visit("/case-search-results?urn=12AB1111111");
      cy.findByTestId("link-12AB1111111").click();

      cy.findAllByTestId("link-back-link").click();

      cy.location("pathname").should("eq", "/case-search-results");
      cy.location("search").should("eq", "?urn=12AB1111111");
    });
  });

  describe("case details", () => {
    it("can show case details", () => {
      cy.visit("/case-details/13401");
      cy.findByTestId("txt-defendant-name").contains("Walsh, Steve");
      cy.findByTestId("txt-case-urn").contains("99ZZ9999999");
    });
  });
});

export {};

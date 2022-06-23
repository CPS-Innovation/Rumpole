describe("Case Details Search", () => {
  describe("Search box", () => {
    it("can search with an empty search term", () => {
      cy.visit("/case-details/13401");

      cy.findByTestId("btn-search-case").should("not.be.disabled");
      cy.findByTestId("div-search-results").should("not.exist");

      cy.findByTestId("btn-search-case").click();

      cy.findByTestId("div-modal").should("exist");
    });

    it("can search only if there is text in the box", () => {
      cy.visit("/case-details/13401");

      cy.findByTestId("input-search-case").type("a");
      cy.findByTestId("btn-search-case").click();

      cy.findByTestId("div-modal").should("exist");
    });

    it("can search via the enter button", () => {
      cy.visit("/case-details/13401");

      cy.findByTestId("input-search-case").type("{enter}");

      cy.findByTestId("div-modal").should("exist");
    });
  });
});

export {};

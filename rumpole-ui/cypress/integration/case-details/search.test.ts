describe("Case Details Search", () => {
  describe("Search box", () => {
    it("can search only if there is text in the box", () => {
      cy.visit("/case-details/13401");

      // assert disabled behaviour
      cy.findByTestId("btn-search-case").should("be.disabled");
      // can't test click as not possible if disabled...

      cy.findByTestId("input-search-case").type("{enter}");
      cy.wait(50); // todo: smell?
      cy.findByTestId("div-search-results").should("not.exist");

      // now type
      cy.findByTestId("input-search-case").type("a");

      // assert enabled behaviour

      // test click
      cy.findByTestId("btn-search-case").should("not.be.disabled").click();

      cy.findByTestId("div-modal").should("exist");

      // reset
      cy.findByTestId("btn-modal-close").click();

      // test enter button
      cy.findByTestId("div-modal").should("not.exist");
      cy.findByTestId("input-search-case").type("{enter}");
      cy.findByTestId("div-modal").should("exist");
    });
  });
});

export {};

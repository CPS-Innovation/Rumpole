describe("searching for cases", () => {
  it("can search for URNs", () => {
    cy.visit("/search");

    cy.findByTestId("input-search-urn").type("12AB1212121");

    cy.findByTestId("button-search").click();

    cy.findAllByTestId("element-result");
  });
});

export {};

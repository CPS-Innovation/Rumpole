describe("the app", () => {
  it("opens", () => {
    cy.visit("/");
    cy.contains("Rumpole");
  });
});

export {};

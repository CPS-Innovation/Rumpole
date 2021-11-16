describe("the app", () => {
  it("opens", () => {
    cy.visit("/");

    cy.contains("Rumpole");
    // home route redirects to /search
    cy.location("pathname").should("equal", "/search");
  });
});

export {};

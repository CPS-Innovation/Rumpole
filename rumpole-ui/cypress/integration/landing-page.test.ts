describe("the app", () => {
  it("opens", () => {
    cy.visit("/");

    //cy.contains("Rumpole");
    // home route redirects to /case-search
    cy.location("pathname").should("equal", "/case-search");
    cy.contains("Find a case");
  });
});

export {};

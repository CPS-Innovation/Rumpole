describe("case details page", () => {
  describe("case page navigation", () => {
    it("can navigate back from case page, having not previously visited results page, and land on search page", () => {
      cy.visit("/case-details/13401");

      cy.findAllByTestId("link-back-link").should("have.attr", "href", "/");

      cy.findAllByTestId("link-back-link").click();
      cy.location("pathname").should("eq", "/case-search");
    });

    it("can navigate back from case page, having previously visited results page, and land on results page", () => {
      cy.visit("/case-search-results?urn=12AB1111111");
      cy.findByTestId("link-12AB1111111").click();

      cy.findAllByTestId("link-back-link").should(
        "have.attr",
        "href",
        "/case-search-results?urn=12AB1111111"
      );

      cy.findAllByTestId("link-back-link").click();

      cy.location("pathname").should("eq", "/case-search-results");
      cy.location("search").should("eq", "?urn=12AB1111111");
    });

    it("shows the unhandled error page if an unexpected error occurrs with the api", () => {
      cy.visitPageAndbreakApiRoute(
        "/case-search-results?urn=12AB1111111",
        "api/case-details/:id",
        500
      );

      cy.findByTestId("link-12AB1111111").click();

      // we are showing the error page
      cy.findByTestId("txt-error-page-heading");
    });
  });

  describe("case details", () => {
    it("can show case details", () => {
      cy.visit("/case-search-results?urn=12AB1111111");
      cy.visit("/case-details/13401");
      cy.findByTestId("txt-defendant-name").contains("Walsh, Steve");
      cy.findByTestId("txt-case-urn").contains("12AB1111111");
    });
  });

  describe("pdf viewing", () => {
    it("can open a pdf", () => {
      cy.visit("/case-search-results?urn=12AB1111111");
      cy.visit("/case-details/13401");
      cy.findByTestId("btn-accordion-open-close-all").click();

      cy.findByTestId("div-pdfviewer").should("not.exist");

      cy.findByTestId("link-document-d1").click();

      cy.findByTestId("div-pdfviewer").should("exist").contains("CASE OUTLINE");
    });
  });
});

export {};

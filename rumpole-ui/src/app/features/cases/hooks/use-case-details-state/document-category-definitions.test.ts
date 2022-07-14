import { CaseDocument } from "../../domain/CaseDocument";
import {
  categoryNamesInPresentationOrder,
  getCategory,
} from "./document-category-definitions";

describe("documentCategoryDefinitions", () => {
  it("there are some categories", () => {
    expect(categoryNamesInPresentationOrder.length).toBeGreaterThan(0);
  });

  it("can resolve a documents category to Uncategorised if prior cateogries match", () => {
    const result = getCategory({} as CaseDocument);

    expect(result).toBe("Uncategorised");
  });
});

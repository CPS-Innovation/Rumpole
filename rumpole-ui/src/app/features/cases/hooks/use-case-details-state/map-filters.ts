import { CombinedState } from "../../domain/CombinedState";
import { MappedTextSearchResult } from "../../domain/MappedTextSearchResult";
import { categoryNamesInPresentationOrder } from "./document-category-definitions";

export const mapFilters = (
  mappedTextSearchResult: MappedTextSearchResult
): CombinedState["searchState"]["filterOptions"] => {
  // we show categories even if they are empty, so kick off with map of all the
  //  categories
  const category = categoryNamesInPresentationOrder.reduce(
    (accumulator, categoryName) => {
      accumulator[categoryName] = {
        label: categoryName,
        count: 0,
        isSelected: false,
      };
      return accumulator;
    },
    {} as CombinedState["searchState"]["filterOptions"]["category"]
  );

  const orderedDocumentResults = mappedTextSearchResult.documentResults.sort(
    (a, b) =>
      a.cmsDocType.name < b.cmsDocType.name
        ? -1
        : a.cmsDocType.name > b.cmsDocType.name
        ? 1
        : 0
  );

  const docType =
    {} as CombinedState["searchState"]["filterOptions"]["docType"];

  for (var doc of orderedDocumentResults) {
    if (!docType[doc.cmsDocType.code]) {
      docType[doc.cmsDocType.code] = {
        label: doc.cmsDocType.name,
        count: 0,
        isSelected: false,
      };
    }

    docType[doc.cmsDocType.code].count += 1;
    category[doc.category].count += 1;
  }

  return {
    category,
    docType,
  };
};

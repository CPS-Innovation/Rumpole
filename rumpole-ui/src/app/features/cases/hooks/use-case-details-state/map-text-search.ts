import { ApiTextSearchResult } from "../../domain/ApiTextSearchResult";
import { MappedCaseDocument } from "../../domain/MappedCaseDocument";
import { MappedDocumentResult } from "../../domain/MappedDocumentResult";
import { MappedTextSearchResult } from "../../domain/MappedTextSearchResult";
import { areAlphanumericallyEqual } from "../../logic/areAlphanumericallyEqual";

type TDocument = MappedTextSearchResult["documentResults"][number];

export const mapTextSearch = (
  apiResults: ApiTextSearchResult[],
  caseDocuments: MappedCaseDocument[],
  searchTerm: string
): MappedTextSearchResult => {
  let totalOccurrencesCount = 0;

  const apiResultDocuments = apiResults.reduce(
    (accumulator, apiResultDocument) => {
      let documentResult = accumulator.find(
        (mappedResult) =>
          mappedResult.documentId === apiResultDocument.documentId
      );

      if (!documentResult) {
        const baseCaseDocument = caseDocuments.find(
          (caseDocument) =>
            caseDocument.documentId === apiResultDocument.documentId
        );

        documentResult = {
          ...baseCaseDocument,
          occurrencesInDocumentCount: 0,
          occurrences: [],
          isVisible: true,
        } as TDocument;
        accumulator.push(documentResult);
      }

      const { id, pageIndex, words } = apiResultDocument;

      const occurrencesInLine = words
        .filter(
          (word) =>
            !!word.boundingBox && // backend sends null for bounding box if not matched word
            areAlphanumericallyEqual(word.text, searchTerm)
        )
        .map(
          (word) =>
            word.boundingBox ||
            // this || clause keeps typescript happy, by this point we are guaranteed to have an array,
            //  with stuff in rather than null, but typescript doen't think so, and I can't find a
            //  type-guard-y kind of way to convince typescript.
            /* istanbul ignore next */
            []
        );

      const thisOccurrence = {
        id,
        pageIndex,
        contextTextChunks: words.map((word) => ({
          text: word.text,
          isHighlighted: areAlphanumericallyEqual(word.text, searchTerm),
        })),
        occurrencesInLine: occurrencesInLine,
      };

      documentResult.occurrences.push(thisOccurrence);
      documentResult.occurrencesInDocumentCount += occurrencesInLine.length;
      totalOccurrencesCount += occurrencesInLine.length;
      return accumulator;
    },
    [] as MappedDocumentResult[]
  );

  return {
    totalOccurrencesCount: totalOccurrencesCount,
    documentResults: apiResultDocuments,
    filteredOccurrencesCount: totalOccurrencesCount,
    filteredDocumentCount: apiResultDocuments.length,
  };
};

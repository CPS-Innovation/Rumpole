import { ApiTextSearchResult } from "../../domain/ApiTextSearchResult";
import { MappedCaseDocument } from "../../domain/MappedCaseDocument";
import { mapTextSearch } from "./map-text-search";

describe("mapTextSearch", () => {
  it("can map api results to a MappedTextSearchResult", () => {
    const apiResults = [
      {
        id: "1",
        documentId: "1",
        pageIndex: 0,
        words: [
          { boundingBox: [1], text: "foo", confidence: 1 },
          { boundingBox: [1], text: "bar", confidence: 1 },
        ],
      },
      {
        id: "2",
        documentId: "2",
        pageIndex: 0,
        words: [
          { boundingBox: [1], text: "baz", confidence: 1 },
          { boundingBox: [1], text: "foo", confidence: 1 },
        ],
      },
      {
        id: "3",
        documentId: "2",
        pageIndex: 0,
        words: [
          { boundingBox: [1], text: "foo", confidence: 1 },
          { boundingBox: [1], text: "foo", confidence: 1 },
        ],
      },
      {
        id: "4",
        documentId: "2",
        pageIndex: 1,
        words: [
          { boundingBox: [1], text: "foo", confidence: 1 },
          { boundingBox: [1], text: "baz", confidence: 1 },
        ],
      },
    ] as ApiTextSearchResult[];
    const caseDocuments = [
      { documentId: "1" },
      { documentId: "2" },
    ] as MappedCaseDocument[];
    const searchTerm = "foo";

    const result = mapTextSearch(apiResults, caseDocuments, searchTerm);

    expect(result).toEqual({
      documentResults: [
        {
          documentId: "1",
          isVisible: true,
          occurrences: [
            {
              contextTextChunks: [
                {
                  isHighlighted: true,
                  text: "foo",
                },
                {
                  isHighlighted: false,
                  text: "bar",
                },
              ],
              id: "1",
              occurrencesInLine: [
                {
                  boundingBox: [1],
                },
              ],
              pageIndex: 0,
            },
          ],
          occurrencesInDocumentCount: 1,
        },
        {
          documentId: "2",
          isVisible: true,
          occurrences: [
            {
              contextTextChunks: [
                {
                  isHighlighted: false,
                  text: "baz",
                },
                {
                  isHighlighted: true,
                  text: "foo",
                },
              ],
              id: "2",
              occurrencesInLine: [
                {
                  boundingBox: [1],
                },
              ],
              pageIndex: 0,
            },
            {
              contextTextChunks: [
                {
                  isHighlighted: true,
                  text: "foo",
                },
                {
                  isHighlighted: true,
                  text: "foo",
                },
              ],
              id: "3",
              occurrencesInLine: [
                {
                  boundingBox: [1],
                },
                {
                  boundingBox: [1],
                },
              ],
              pageIndex: 0,
            },
            {
              contextTextChunks: [
                {
                  isHighlighted: true,
                  text: "foo",
                },
                {
                  isHighlighted: false,
                  text: "baz",
                },
              ],
              id: "4",
              occurrencesInLine: [
                {
                  boundingBox: [1],
                },
              ],
              pageIndex: 1,
            },
          ],
          occurrencesInDocumentCount: 4,
        },
      ],
      filteredDocumentCount: 2,
      filteredOccurrencesCount: 5,
      totalOccurrencesCount: 5,
    });
  });
});

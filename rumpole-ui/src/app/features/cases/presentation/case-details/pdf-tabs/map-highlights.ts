import { CaseDocumentViewModel } from "../../../domain/CaseDocumentViewModel";
import { IHighlight } from "../../../../../../react-pdf-highlighter";

const PDF_HEIGHT_INCHES = 11.69; //todo: this needs to come from caseDocumentParameter
const PDF_WIDTH_INCHES = 8.27;
const PADDING_INCHES = 0.03;

export const mapHighlights = ({
  pageOccurrences,
  searchTerm,
}: Extract<CaseDocumentViewModel, { mode: "search" }>): IHighlight[] => {
  type NewType = IHighlight;

  const results: NewType[] = [];

  let i = 0;
  for (const { boundingBoxes, pageIndex } of pageOccurrences) {
    for (const [x1, y1, , , x2, y2] of boundingBoxes) {
      const rect = {
        x1: x1 - PADDING_INCHES,
        y1: y1 - PADDING_INCHES,
        x2: x2 + PADDING_INCHES,
        y2: y2 + PADDING_INCHES,
        width: PDF_WIDTH_INCHES,
        height: PDF_HEIGHT_INCHES,
      };
      results.push({
        id: String(i++),
        content: { text: searchTerm },
        comment: { text: "", emoji: "" },
        position: {
          pageNumber: pageIndex,
          boundingRect: rect,
          rects: [rect],
        },
      });
    }
  }
  return results;
};
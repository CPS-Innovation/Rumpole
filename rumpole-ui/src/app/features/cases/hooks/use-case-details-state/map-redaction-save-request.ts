import { IPdfHighlight } from "../../domain/IPdfHighlight";
import { RedactionSavePage } from "../../domain/RedactionSavePage";
import { RedactionSaveRequest } from "../../domain/RedactionSaveRequest";

export const mapRedactionSaveRequest = (
  pdfId: string,
  redactionHighlights: IPdfHighlight[]
) => {
  const redactionSaveRequest = redactionHighlights.reduce(
    (acc, curr) => {
      let redactionPage = acc.redactionPages.find(
        (item) => item.pageIndex === curr.position.pageNumber
      );

      if (!redactionPage) {
        const { pageNumber: pageIndex } = curr.position;
        const { height, width } = curr.position.boundingRect;

        redactionPage = {
          pageIndex,
          height,
          width,
          redactionCoordinates: [],
        };
        acc.redactionPages.push(redactionPage);
      }

      redactionPage.redactionCoordinates.push(
        ...curr.position.rects.map((item) => ({
          x1: item.x1,
          y1: item.y1,
          x2: item.x2,
          y2: item.y2,
        }))
      );

      return acc;
    },
    {
      docId: +pdfId,
      redactionPages: [] as RedactionSavePage[],
    } as RedactionSaveRequest
  );

  return redactionSaveRequest;
};

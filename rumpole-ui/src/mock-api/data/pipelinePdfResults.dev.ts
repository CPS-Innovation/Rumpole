import { PdfDocument } from "../../app/features/cases/domain/PdfDocument";
import { PipelineResults } from "../../app/features/cases/domain/PipelineResults";
import { PipelinePdfResultsDataSource } from "./types/PipelinePdfResultsDataSource";

let callIndex = 0;
const dataSource: PipelinePdfResultsDataSource = () => {
  if (callIndex > pipelinePdfResults.documents.length) {
    callIndex = 0;
  }

  console.log(callIndex);
  const incrementingData = pipelinePdfResults.documents.reduce(
    (prev, curr, index) => {
      if (index >= callIndex) {
        // whilst callIndex grows, any documents at positions greater or equal
        // to callIndex will return a not ready record (empty pdfBlobName)
        return [...prev, { ...curr, pdfBlobName: "" }];
      } else {
        return [...prev, curr];
      }
    },
    [] as PdfDocument[]
  );

  callIndex += 1;
  return { ...pipelinePdfResults, documents: incrementingData };
};

export default dataSource;

const pipelinePdfResults: PipelineResults = {
  transationId: "121",
  documents: [
    {
      documentId: "d1",
      pdfBlobName: "MCLOVEMG3",
      status: "finished",
    },
    {
      documentId: "d2",
      pdfBlobName: "CM01",
      status: "finished",
    },
    {
      documentId: "d3",
      pdfBlobName: "MG05MCLOVE",
      status: "finished",
    },
    {
      documentId: "d4",
      pdfBlobName: "MG06_3June",
      status: "finished",
    },
    {
      documentId: "d5",
      pdfBlobName: "MG06_10june",
      status: "finished",
    },
  ],
};

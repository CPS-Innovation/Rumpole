import { PdfDocument } from "../../app/features/cases/domain/PdfDocument";
import { PipelineResults } from "../../app/features/cases/domain/PipelineResults";
import { PipelinePdfResultsDataSource } from "./types/PipelinePdfResultsDataSource";

let callIndex = 0;
const dataSource: PipelinePdfResultsDataSource = () => {
  if (callIndex > pipelinePdfResults.documents.length) {
    callIndex = 0;
  }

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
  transactionId: "121",
  status: "Completed",
  documents: [
    {
      documentId: "d1",
      pdfBlobName: "MCLOVEMG3",
      status: "Indexed",
    },
    {
      documentId: "d2",
      pdfBlobName: "CM01",
      status: "Indexed",
    },
    {
      documentId: "d3",
      pdfBlobName: "MG05MCLOVE",
      status: "Indexed",
    },
    {
      documentId: "d4",
      pdfBlobName: "MG06_3June",
      status: "Indexed",
    },
    {
      documentId: "d5",
      pdfBlobName: "MG06_10june",
      status: "Indexed",
    },
    {
      documentId: "d6",
      pdfBlobName: "MCLOVEMG3",
      status: "Indexed",
    },
    {
      documentId: "d7",
      pdfBlobName: "CM01",
      status: "Indexed",
    },
    {
      documentId: "d8",
      pdfBlobName: "MG05MCLOVE",
      status: "Indexed",
    },
    {
      documentId: "d9",
      pdfBlobName: "MG06_3June",
      status: "Indexed",
    },
    {
      documentId: "d10",
      pdfBlobName: "MG06_10june",
      status: "Indexed",
    },
  ],
};

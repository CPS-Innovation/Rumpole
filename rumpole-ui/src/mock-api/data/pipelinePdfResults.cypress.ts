import { PipelineResults } from "../../app/features/cases/domain/PipelineResults";
import { PipelinePdfResultsDataSource } from "./types/PipelinePdfResultsDataSource";

const dataSource: PipelinePdfResultsDataSource = () => pipelinePdfResults;

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
  ],
};

export const missingDocsPipelinePdfResults: PipelineResults = {
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
      status: "OcrAndIndexFailure",
    },
    {
      documentId: "d5",
      pdfBlobName: "MG06_10june",
      status: "UnableToConvertToPdf",
    },
  ],
};

export const allMissingDocsPipelinePdfResults: PipelineResults = {
  transactionId: "121",
  status: "Completed",
  documents: [
    {
      documentId: "d1",
      pdfBlobName: "MCLOVEMG3",
      status: "OcrAndIndexFailure",
    },
    {
      documentId: "d2",
      pdfBlobName: "CM01",
      status: "OcrAndIndexFailure",
    },
    {
      documentId: "d3",
      pdfBlobName: "MG05MCLOVE",
      status: "OcrAndIndexFailure",
    },
    {
      documentId: "d4",
      pdfBlobName: "MG06_3June",
      status: "OcrAndIndexFailure",
    },
    {
      documentId: "d5",
      pdfBlobName: "MG06_10june",
      status: "UnableToConvertToPdf",
    },
  ],
};

import { PipelineResults } from "../../app/features/cases/domain/PipelineResults";
import { PipelinePdfResultsDataSource } from "./types/PipelinePdfResultsDataSource";

const dataSource: PipelinePdfResultsDataSource = () => pipelinePdfResults;

export default dataSource;

const pipelinePdfResults: PipelineResults = {
  transationId: "121",
  documents: [
    {
      documentId: "d1",
      pdfBlobName: "CM01",
      status: "finished",
    },
  ],
};

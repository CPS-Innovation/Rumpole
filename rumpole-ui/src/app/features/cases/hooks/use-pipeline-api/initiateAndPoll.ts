import { ApiError } from "../../../../common/errors/ApiError";
import { ApiResult } from "../../../../common/types/ApiResult";
import { getPipelinePdfResults, initiatePipeline } from "../../api/gateway-api";
import { PipelineResults } from "../../domain/PipelineResults";

const delay = (delayMs: number) =>
  new Promise((resolve) => setTimeout(resolve, delayMs));

export const initiateAndPoll = (
  caseId: string,
  delayMs: number,
  del: (pipelineResults: ApiResult<PipelineResults>) => void
) => {
  let keepPolling = true;

  const handleError = (error: any) => {
    keepPolling = false;
    del({
      status: "failed",
      error,
      httpStatusCode: error instanceof ApiError ? error.code : undefined,
    });
  };

  const handlePipelineSuccess = (pipelineResult: PipelineResults) => {
    del({
      status: "succeeded",
      data: pipelineResult,
    });

    if (pipelineResult.documents.every((doc) => doc.pdfBlobName)) {
      keepPolling = false;
    }
  };

  const doWork = async () => {
    let trackerUrl = "";

    try {
      trackerUrl = await initiatePipeline(caseId);
    } catch (error) {
      handleError(error);
    }

    while (keepPolling) {
      try {
        await delay(delayMs);

        const pipelineResult = await getPipelinePdfResults(trackerUrl);
        handlePipelineSuccess(pipelineResult);
      } catch (error) {
        handleError(error);
      }
    }
  };

  doWork();

  return () => {
    // allow consumer to kill loop
    keepPolling = false;
  };
};

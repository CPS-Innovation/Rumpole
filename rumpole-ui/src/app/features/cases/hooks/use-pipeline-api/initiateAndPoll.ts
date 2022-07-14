import { ApiError } from "../../../../common/errors/ApiError";
import { AsyncPipelineResult } from "./AsyncPipelineResult";
import { getPipelinePdfResults, initiatePipeline } from "../../api/gateway-api";
import { PipelineResults } from "../../domain/PipelineResults";
import { isPipelineFinished } from "../../domain/PipelineStatus";

const delay = (delayMs: number) =>
  new Promise((resolve) => setTimeout(resolve, delayMs));

export const initiateAndPoll = (
  caseId: string,
  delayMs: number,
  del: (pipelineResults: AsyncPipelineResult<PipelineResults>) => void
) => {
  let keepPolling = true;

  const handleSuccess = (pipelineResult: PipelineResults) => {
    const isComplete = isPipelineFinished(pipelineResult.status);
    del({
      status: isComplete ? "complete" : "incomplete",
      data: pipelineResult,
      haveData: true,
    });

    if (isComplete) {
      keepPolling = false;
    }
  };

  const handleError = (error: any) => {
    del({
      status: "failed",
      error,
      httpStatusCode: error instanceof ApiError ? error.code : undefined,
      haveData: false,
    });

    keepPolling = false;
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
        handleSuccess(pipelineResult);
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

import { useEffect, useState } from "react";
import { AsyncPipelineResult } from "./AsyncPipelineResult";
import { PipelineResults } from "../../domain/PipelineResults";
import { initiateAndPoll } from "./initiateAndPoll";
import { PIPELINE_POLLING_DELAY } from "../../../../config";

export const usePipelineApi = (
  caseId: string
): AsyncPipelineResult<PipelineResults> => {
  const [pipelineResults, setPipelineResults] = useState<
    AsyncPipelineResult<PipelineResults>
  >({
    status: "initiating",
    haveData: false,
  });

  useEffect(() => {
    return initiateAndPoll(caseId, PIPELINE_POLLING_DELAY, (results) =>
      setPipelineResults(results)
    );
  }, [caseId]);

  return pipelineResults;
};

import { useEffect, useState } from "react";
import { ApiResult } from "../../../../common/types/ApiResult";
import { PipelineResults } from "../../domain/PipelineResults";
import { initiateAndPoll } from "./initiateAndPoll";

const POLLING_DELAY = 1000;

export const usePipelineApi = (caseId: string): ApiResult<PipelineResults> => {
  const [pipelineResults, setPipelineResults] = useState<
    ApiResult<PipelineResults>
  >({
    status: "loading",
  });

  useEffect(() => {
    return initiateAndPoll(caseId, POLLING_DELAY, (results) =>
      setPipelineResults(results)
    );
  }, [caseId]);

  return pipelineResults;
};

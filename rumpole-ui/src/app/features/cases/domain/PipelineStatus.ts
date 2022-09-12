const InProgressPipelineStatusesArray = [
  "NotStarted",
  "Running",
  "NoDocumentsFoundInCDE",
  "Completed",
  "Failed",
] as const;

const SummaryPipelineStatusesArray = [
  "Running",
  "Completed",
  "Failed",
] as const;

const pipelineFailedStatuses: InProgressPipelineStatus[] = ["Failed"];

const pipelineSucceededStatuses: InProgressPipelineStatus[] = [
  "NoDocumentsFoundInCDE",
  "Completed",
];

export const getPipelinpipelineCompletionStatus = (
  status: InProgressPipelineStatus
): SummaryPipelineStatus => {
  if (pipelineSucceededStatuses.includes(status)) {
    return "Completed";
  }
  if (pipelineFailedStatuses.includes(status)) {
    return "Failed";
  }
  return "Running";
};

type PipelineStatusesTuple = typeof InProgressPipelineStatusesArray;

export type InProgressPipelineStatus = PipelineStatusesTuple[number];

type SummaryPipelineStatusesTuple = typeof SummaryPipelineStatusesArray;

export type SummaryPipelineStatus = SummaryPipelineStatusesTuple[number];

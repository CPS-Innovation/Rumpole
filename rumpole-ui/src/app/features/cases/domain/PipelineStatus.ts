export const PipelineStatuses = [
  "NotStarted",
  "Running",
  "NoDocumentsFoundInCDE",
  "Completed",
  "Failed",
] as const;

export const isPipelineFinished = (status: PipelineStatus) => {
  return pipelineFinishedStatuses.includes(status);
};

const pipelineFinishedStatuses: PipelineStatus[] = [
  "NoDocumentsFoundInCDE",
  "Completed",
  "Failed",
];

type PipelineStatusesTuple = typeof PipelineStatuses;

export type PipelineStatus = PipelineStatusesTuple[number];

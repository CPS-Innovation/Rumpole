type CaseStatus = "open" | "finalised" | "charged" | "not-yet-charged";

export type UrnFilterQueryParams = {
  areas?: string[];
  status?: CaseStatus;
  agency?: string;
};

type CaseStatus = "open" | "finalised" | "charged" | "not-yet-charged";

export type CaseFilterQueryParams = {
  urn: string;
  area: string[] | undefined;
  status: CaseStatus | undefined;
  agency: string[] | undefined;
};

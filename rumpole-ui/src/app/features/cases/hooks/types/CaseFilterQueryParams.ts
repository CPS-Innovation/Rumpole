export type CaseFilterQueryParams = {
  urn: string;
  area: string[] | undefined;
  status: "open" | "finalised" | "charged" | "not-yet-charged" | undefined;
  chargedStatus: "true" | "false" | undefined;
  agency: string[] | undefined;
};

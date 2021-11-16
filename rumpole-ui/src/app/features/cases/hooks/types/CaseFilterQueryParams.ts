export type CaseFilterQueryParams = {
  urn: string;
  area: string[] | undefined;
  chargedStatus: "true" | "false" | undefined;
  agency: string[] | undefined;
};

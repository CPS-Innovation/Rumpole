const stripNonAlphanumeric = (str: string) =>
  str.replace(/[.,/#!$%^&*;:{}=\-_`~()…”]/g, "").replace(/\s{2,}/g, " ");

export const areAlphanumericallyEqual = (a: string, b: string) =>
  stripNonAlphanumeric(a).localeCompare(stripNonAlphanumeric(b), undefined, {
    sensitivity: "accent",
  }) === 0;

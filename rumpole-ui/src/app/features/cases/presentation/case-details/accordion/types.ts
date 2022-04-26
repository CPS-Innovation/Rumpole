export type AccordionDocument = {
  docId: string;
  docLabel: string;
  docDate: string;
};

export type Section = {
  sectionId: string;
  sectionLabel: string;
  docs: AccordionDocument[];
};

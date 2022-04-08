export type CaseSearchResult = {
  id: number;
  uniqueReferenceNumber: string;
  area: {
    code: string;
    name: string;
  };
  investigativeAgency: {
    code: string;
    name: string;
  };
  leadDefendant: {
    firstNames: string;
    surname: string;
    organisationName: string;
  };
  offences: {
    code: string;
    earlyDate: string;
    lateDate: string;
    listOrder: number;
    shortDescription: string;
    longDescription: string;
    isNotYetCharged: boolean;
  }[];
};

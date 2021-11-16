export type CaseSearchResult = {
  uniqueReferenceNumber: string;
  id: number;
  area: {
    code: string;
    name: string;
  };
  agency: {
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

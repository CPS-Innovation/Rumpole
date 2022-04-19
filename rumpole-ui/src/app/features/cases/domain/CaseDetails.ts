export type CaseDetails = {
  id: number;
  uniqueReferenceNumber: string;
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

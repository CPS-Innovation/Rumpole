export type CaseDetails = {
  id: number;
  uniqueReferenceNumber: string;
  isCaseCharged: boolean;
  leadDefendantDetails: DefendantDetails;
  headlineCharge: Charge;
  defendants: {
    defendantDetails: DefendantDetails;
    custodyTimeLimit: CustodyTimeLimit;
    charges: Charge[];
  };
};

type DefendantDetails = {
  id: number;
  listOrder: number;
  firstNames: string;
  surname: string;
  organisationName: string;
  dob: string;
  isYouth: boolean;
};

type Charge = {
  id: number;
  listOrder: number;
  isCharged: boolean;
  nextHearingDate: string;
  earlyDate: string;
  lateDate: string;
  code: string;
  shortDescription: string;
  longDescription: string;
  custodyTimeLimit: CustodyTimeLimit;
};

type CustodyTimeLimit = {
  expiryDate: string;
  expiryDays: number;
  expiryIndicator: string;
};

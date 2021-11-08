export type CaseSearchResult = {
  id: number;
  urn: string;
  isCharged: boolean;
  area: {
    code: string;
    name: string;
  };
  status: {
    code: string;
    description: string;
  };
  agency: {
    code: string;
    name: string;
  };
  leadDefendant: {
    firstNames?: string;
    surname?: string;
    organisationName?: string;
  };
  offences: {
    earlyDate: string;
    lateDate: string;
    listOrder: number;
    code: string;
    shortDescription: string;
    longDescription: string;
  }[];
};

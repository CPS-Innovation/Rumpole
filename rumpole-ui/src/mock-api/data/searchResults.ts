import { CaseSearchResult } from "../../app/features/cases/domain/CaseSearchResult";
import { Agency } from "./Agency";
import { AreaDivision } from "./AreaDivision";
import { Status } from "./Status";

type CoreCaseSearchResult = Omit<CaseSearchResult, "urn">;

export const searchResults: CoreCaseSearchResult[] = [
  {
    id: 13401,
    status: Status.Charged,
    area: AreaDivision.LondonSouth,
    agency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: "Short description",
        longDescription: "Long description",
      },
    ],
  },
  {
    id: 17422,
    status: Status.Charged,
    area: AreaDivision.ThamesAndChiltern,
    agency: Agency.SurreyPolice,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: "Short description",
        longDescription: "Long description",
      },
    ],
  },
  {
    id: 18443,
    status: Status.Charged,
    area: AreaDivision.EastOfEngland,
    agency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: "Short description",
        longDescription: "Long description",
      },
    ],
  },
  {
    id: 19994,
    status: Status.Finalised,
    area: AreaDivision.ThamesAndChiltern,
    agency: Agency.SurreyPolice,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: "Short description",
        longDescription: "Long description",
      },
    ],
  },
  {
    id: 17425,
    status: Status.Charged,
    area: AreaDivision.LondonSouth,
    agency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: "Short description",
        longDescription: "Long description",
      },
    ],
  },
  {
    id: 16756,
    status: Status.Charged,
    area: AreaDivision.LondonSouth,
    agency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: "Short description",
        longDescription: "Long description",
      },
    ],
  },
  {
    id: 14927,
    status: Status.Charged,
    area: AreaDivision.LondonSouth,
    agency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: "Short description",
        longDescription: "Long description",
      },
    ],
  },
  {
    id: 17428,
    status: Status.Charged,
    area: AreaDivision.LondonSouth,
    agency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: "Short description",
        longDescription: "Long description",
      },
    ],
  },
  {
    id: 16549,
    status: Status.Charged,
    area: AreaDivision.LondonSouth,
    agency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: "Short description",
        longDescription: "Long description",
      },
    ],
  },
];

import { Agency } from "../lookups/Agency";
import { AreaDivision } from "../lookups/AreaDivision";
import { Status } from "../lookups/Status";
import { CaseSearchResult } from "../../app/features/cases/domain/CaseSearchResult";
import { SearchDataSource } from "./types/SearchDataSource";

const dataSource: SearchDataSource = (urn) =>
  searchResults.filter((item) => item.urn === urn);

export default dataSource;

export const searchResults: CaseSearchResult[] = [
  {
    urn: "52AB1111111",
    id: 13401,
    isCharged: false,
    status: Status.Charged,
    area: AreaDivision.LondonSouth,
    agency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Adam",
      surname: "Apple",
    },
    offences: [],
  },
  {
    urn: "36AB1111112",
    id: 13402,
    isCharged: true,
    status: Status.Charged,
    area: AreaDivision.NorthWest,
    agency: Agency.CumbriaConstabulary,
    leadDefendant: {
      firstNames: "Berty",
      surname: "Banana",
    },
    offences: [
      {
        earlyDate: "2021-10-31",
        lateDate: "2021-10-31",
        listOrder: 0,
        code: "0",
        shortDescription: "Bladed article",
        longDescription: "Bladed article",
      },
    ],
  },
  {
    urn: "48AB1111113",
    id: 13403,
    isCharged: false,
    status: Status.Charged,
    area: AreaDivision.NorthWest,
    agency: Agency.LancashireConstabulary,
    leadDefendant: {
      firstNames: "Chad",
      surname: "Carrot",
    },
    offences: [],
  },
  {
    urn: "51AB1111114",
    id: 13402,
    isCharged: true,
    status: Status.Charged,
    area: AreaDivision.MerseryCheshire,
    agency: Agency.MerseysidePolice,
    leadDefendant: {
      firstNames: "Dan",
      surname: "Dill",
    },
    offences: [
      {
        earlyDate: "2021-09-28",
        lateDate: "2021-09-28",
        listOrder: 0,
        code: "0",
        shortDescription: "Dangerous driving",
        longDescription: "Dangerous driving",
      },
    ],
  },
  {
    urn: "43AB1111115",
    id: 13402,
    isCharged: true,
    status: Status.Charged,
    area: AreaDivision.MerseryCheshire,
    agency: Agency.MerseysidePolice,
    leadDefendant: {
      firstNames: "Edward",
      surname: "Egg",
    },
    offences: [
      {
        earlyDate: "2021-11-01",
        lateDate: "2021-11-01",
        listOrder: 0,
        code: "0",
        shortDescription: "s.39",
        longDescription: "s.39",
      },
    ],
  },
];

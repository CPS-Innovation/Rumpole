import { Agency } from "../lookups/Agency";
import { AreaDivision } from "../lookups/AreaDivision";
import { Status } from "../lookups/Status";
import faker from "faker";
import { CaseSearchResult } from "../../app/features/cases/domain/CaseSearchResult";
import { SearchDataSource } from "./types/SearchDataSource";
const dataSource: SearchDataSource = (urn) => {
  const lastDigit = Number(urn?.split("").pop());

  const coreResults = lastDigit ? [...searchResults].slice(-1 * lastDigit) : [];

  return coreResults.map((result) => ({
    ...result,
    urn,
  })) as CaseSearchResult[];
};

export default dataSource;

const searchResults: Omit<CaseSearchResult, "urn">[] = [
  {
    id: 13401,
    isCharged: true,
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
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
      },
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
      },
    ],
  },
  {
    id: 17422,
    isCharged: true,
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
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
      },
    ],
  },
  {
    id: 18443,
    isCharged: false,
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
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
      },
    ],
  },
  {
    id: 19994,
    isCharged: false,
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
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
      },
    ],
  },
  {
    id: 17425,
    isCharged: true,
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
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
      },
    ],
  },
  {
    id: 16756,
    isCharged: false,
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
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
      },
    ],
  },
  {
    id: 14927,
    isCharged: true,
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
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
      },
    ],
  },
  {
    id: 17428,
    isCharged: false,
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
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
      },
    ],
  },
  {
    id: 16549,
    isCharged: true,
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
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
      },
    ],
  },
];

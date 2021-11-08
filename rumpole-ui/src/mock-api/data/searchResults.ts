import { CaseSearchResult } from "../../app/features/cases/domain/CaseSearchResult";
import { Agency } from "./Agency";
import { AreaDivision } from "./AreaDivision";
import { Status } from "./Status";
import faker from "faker";

type CoreCaseSearchResult = Omit<CaseSearchResult, "urn">;

export const searchResults: CoreCaseSearchResult[] = [
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

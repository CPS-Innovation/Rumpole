import { Agency } from "../lookups/Agency";
import { AreaDivision } from "../lookups/AreaDivision";
import faker from "faker";
import { CaseSearchResult } from "../../app/features/cases/domain/CaseSearchResult";
import { SearchDataSource } from "./types/SearchDataSource";

const dataSource: SearchDataSource = (urn) => {
  const lastDigit = Number(urn?.split("").pop());

  const coreResults = lastDigit ? [...searchResults].slice(-1 * lastDigit) : [];

  return coreResults.map((result) => ({
    ...result,
    uniqueReferenceNumber: urn,
  })) as CaseSearchResult[];
};

export default dataSource;

const searchResults: Omit<CaseSearchResult, "uniqueReferenceNumber">[] = [
  {
    id: 13401,

    area: AreaDivision.LondonSouth,
    investigativeAgency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
      organisationName: "",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
        isNotYetCharged: false,
      },
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
        isNotYetCharged: true,
      },
    ],
  },
  {
    id: 17422,

    area: AreaDivision.ThamesAndChiltern,
    investigativeAgency: Agency.SurreyPolice,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
      organisationName: "",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
        isNotYetCharged: true,
      },
    ],
  },
  {
    id: 18443,

    area: AreaDivision.EastOfEngland,
    investigativeAgency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
      organisationName: "",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
        isNotYetCharged: true,
      },
    ],
  },
  {
    id: 19994,

    area: AreaDivision.ThamesAndChiltern,
    investigativeAgency: Agency.SurreyPolice,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
      organisationName: "",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
        isNotYetCharged: false,
      },
    ],
  },
  {
    id: 17425,

    area: AreaDivision.LondonSouth,
    investigativeAgency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
      organisationName: "",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
        isNotYetCharged: true,
      },
    ],
  },
  {
    id: 16756,

    area: AreaDivision.LondonSouth,
    investigativeAgency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
      organisationName: "",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
        isNotYetCharged: true,
      },
    ],
  },
  {
    id: 14927,

    area: AreaDivision.LondonSouth,
    investigativeAgency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
      organisationName: "",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
        isNotYetCharged: true,
      },
    ],
  },
  {
    id: 17428,

    area: AreaDivision.LondonSouth,
    investigativeAgency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
      organisationName: "",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
        isNotYetCharged: true,
      },
    ],
  },
  {
    id: 16549,

    area: AreaDivision.LondonSouth,
    investigativeAgency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
      organisationName: "",
    },
    offences: [
      {
        earlyDate: "2020-03-01",
        lateDate: "2021-06-30",
        listOrder: 0,
        code: "0",
        shortDescription: faker.lorem.sentence(),
        longDescription: faker.lorem.sentences(),
        isNotYetCharged: true,
      },
    ],
  },
];

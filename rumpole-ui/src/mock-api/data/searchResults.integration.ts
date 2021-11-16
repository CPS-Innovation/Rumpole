import { Agency } from "../lookups/Agency";
import { AreaDivision } from "../lookups/AreaDivision";
import faker from "faker";
import { CaseSearchResult } from "../../app/features/cases/domain/CaseSearchResult";
import { SearchDataSource } from "./types/SearchDataSource";

const dataSource: SearchDataSource = (urn) =>
  searchResults.filter((item) => item.uniqueReferenceNumber === urn);

export default dataSource;

const searchResults: CaseSearchResult[] = [
  {
    uniqueReferenceNumber: "12AB1111111",
    id: 13401,
    area: AreaDivision.LondonSouth,
    agency: Agency.MetropolitanPoliceService,
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
    uniqueReferenceNumber: "12AB2222222",
    id: 17422,
    area: AreaDivision.ThamesAndChiltern,
    agency: Agency.SurreyPolice,
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
    uniqueReferenceNumber: "12AB3333333",
    id: 18443,
    area: AreaDivision.LondonNorth,
    agency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
      organisationName: "",
    },
    offences: [],
  },
  {
    uniqueReferenceNumber: "12AB3333333",
    id: 18444,
    area: AreaDivision.LondonSouth,
    agency: Agency.CounterFraudAuthority,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
      organisationName: "",
    },
    offences: [],
  },
];

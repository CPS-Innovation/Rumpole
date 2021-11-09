import { Agency } from "../lookups/Agency";
import { AreaDivision } from "../lookups/AreaDivision";
import { Status } from "../lookups/Status";
import faker from "faker";

import { CaseSearchResult } from "../../app/features/cases/domain/CaseSearchResult";
import { SearchDataSource } from "./types/SearchDataSource";

const dataSource: SearchDataSource = (urn) =>
  searchResults.filter((item) => item.urn === urn);

export default dataSource;

const searchResults: CaseSearchResult[] = [
  {
    urn: "12AB1111111",
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
    urn: "12AB2222222",
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
    urn: "12AB3333333",
    id: 18443,
    isCharged: false,
    status: Status.Charged,
    area: AreaDivision.LondonNorth,
    agency: Agency.MetropolitanPoliceService,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
    },
    offences: [],
  },
  {
    urn: "12AB3333333",
    id: 18444,
    isCharged: false,
    status: Status.Charged,
    area: AreaDivision.LondonSouth,
    agency: Agency.CounterFraudAuthority,
    leadDefendant: {
      firstNames: "Steve",
      surname: "Walsh",
    },
    offences: [],
  },
];

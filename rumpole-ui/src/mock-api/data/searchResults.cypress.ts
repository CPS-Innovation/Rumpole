import faker from "faker";

import { CaseSearchResult } from "../../app/features/cases/domain/CaseSearchResult";
import { SearchDataSource } from "./types/SearchDataSource";

const dataSource: SearchDataSource = (urn) =>
  searchResults.filter((item) => item.uniqueReferenceNumber.startsWith(urn));

export default dataSource;

const searchResults: CaseSearchResult[] = [
  // {
  //   uniqueReferenceNumber: "12AB1111111",
  //   id: 13401,
  //   leadDefendant: {
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //   },
  //   offences: [
  //     {
  //       earlyDate: "2020-03-01",
  //       lateDate: "2021-06-30",
  //       listOrder: 0,
  //       code: "0",
  //       shortDescription: faker.lorem.sentence(),
  //       longDescription: faker.lorem.sentences(),
  //       isNotYetCharged: false,
  //     },
  //     {
  //       earlyDate: "2020-03-01",
  //       lateDate: "2021-06-30",
  //       listOrder: 0,
  //       code: "0",
  //       shortDescription: faker.lorem.sentence(),
  //       longDescription: faker.lorem.sentences(),
  //       isNotYetCharged: false,
  //     },
  //   ],
  // },
  // {
  //   uniqueReferenceNumber: "12AB2222222/1",
  //   id: 17422,
  //   leadDefendant: {
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //   },
  //   offences: [
  //     {
  //       earlyDate: "2020-03-01",
  //       lateDate: "2021-06-30",
  //       listOrder: 0,
  //       code: "0",
  //       shortDescription: faker.lorem.sentence(),
  //       longDescription: faker.lorem.sentences(),
  //       isNotYetCharged: false,
  //     },
  //   ],
  // },
  // {
  //   uniqueReferenceNumber: "12AB2222222/2",
  //   id: 18443,
  //   leadDefendant: {
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //   },
  //   offences: [
  //     {
  //       earlyDate: "2020-03-01",
  //       lateDate: "2021-06-30",
  //       listOrder: 0,
  //       code: "0",
  //       shortDescription: faker.lorem.sentence(),
  //       longDescription: faker.lorem.sentences(),
  //       isNotYetCharged: false,
  //     },
  //   ],
  // },
];

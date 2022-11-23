import faker from "faker";
import { CaseDetails } from "../../app/features/cases/domain/CaseDetails";
import { CaseDetailsDataSource } from "./types/CaseDetailsDataSource";

const dataSource: CaseDetailsDataSource = (id) =>
  caseDetails.find((item) => String(item.id) === id);

export default dataSource;

const caseDetails: CaseDetails[] = [
  // {
  //   id: 13401,
  //   uniqueReferenceNumber: "",
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
  //       isNotYetCharged: true,
  //     },
  //   ],
  // },
  // {
  //   id: 17422,
  //   uniqueReferenceNumber: "",
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
  //       isNotYetCharged: true,
  //     },
  //   ],
  // },
  // {
  //   id: 18443,
  //   uniqueReferenceNumber: "",
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
  //       isNotYetCharged: true,
  //     },
  //   ],
  // },
];

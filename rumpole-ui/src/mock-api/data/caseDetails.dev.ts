import faker from "faker";
import { CaseDetails } from "../../app/features/cases/domain/CaseDetails";
import { CaseDetailsDataSource } from "./types/CaseDetailsDataSource";

const dataSource: CaseDetailsDataSource = (id) =>
  caseDetails.find((item) => String(item.id) === id);

export default dataSource;

const caseDetails: CaseDetails[] = [
  //{
  //   id: 13401,
  //   uniqueReferenceNumber: "",
  //   isCaseCharged: true,
  //   leadDefendantDetails: {
  //     id: 1,
  //     listOrder: 0,
  //     youth: false,
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //     dob: "1980-01-01",
  //   },
  //   defendants: [
  //     {
  //       defendantDetails: {
  //         id: 1,
  //         listOrder: 0,
  //         youth: false,
  //         firstNames: "Steve",
  //         surname: "Walsh",
  //         organisationName: "",
  //         dob: "1980-01-01",
  //       },
  //       charges: [{
  //           id: 0;
  // listOrder: 0;
  // isCharged: true;
  // nextHearingDate: "2022-12-31";
  // earlyDate: string;
  // lateDate: string;
  // shortDescription: string;
  // longDescription: string;
  // custodyTimeLimit: CustodyTimeLimit;
  //       }],
  //       custodyTimeLimit: {
  //         expiryDate: "2023-02-02",
  //         expiryDays: 99,
  //         expiryIndicator: "X",
  //       },
  //     },
  //   ],
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
  //   isCaseCharged: true,
  //   leadDefendantDetails: {
  //     id: 1,
  //     listOrder: 0,
  //     youth: false,
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //     dob: "1980-01-01",
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
  //   isCaseCharged: true,
  //   leadDefendantDetails: {
  //     id: 1,
  //     listOrder: 0,
  //     youth: false,
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //     dob: "1980-01-01",
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
  //   id: 19994,
  //   uniqueReferenceNumber: "",
  //   isCaseCharged: true,
  //   leadDefendantDetails: {
  //     id: 1,
  //     listOrder: 0,
  //     youth: false,
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //     dob: "1980-01-01",
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
  //   id: 17425,
  //   uniqueReferenceNumber: "",
  //   isCaseCharged: true,
  //   leadDefendantDetails: {
  //     id: 1,
  //     listOrder: 0,
  //     youth: false,
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //     dob: "1980-01-01",
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
  //   id: 16756,
  //   uniqueReferenceNumber: "",
  //   isCaseCharged: true,
  //   leadDefendantDetails: {
  //     id: 1,
  //     listOrder: 0,
  //     youth: false,
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //     dob: "1980-01-01",
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
  //   id: 14927,
  //   uniqueReferenceNumber: "",
  //   isCaseCharged: true,
  //   leadDefendantDetails: {
  //     id: 1,
  //     listOrder: 0,
  //     youth: false,
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //     dob: "1980-01-01",
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
  //   id: 17428,
  //   uniqueReferenceNumber: "",
  //   isCaseCharged: true,
  //   leadDefendantDetails: {
  //     id: 1,
  //     listOrder: 0,
  //     youth: false,
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //     dob: "1980-01-01",
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
  //   id: 16549,
  //   uniqueReferenceNumber: "",
  //   isCaseCharged: true,
  //   leadDefendantDetails: {
  //     id: 1,
  //     listOrder: 0,
  //     youth: false,
  //     firstNames: "Steve",
  //     surname: "Walsh",
  //     organisationName: "",
  //     dob: "1980-01-01",
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

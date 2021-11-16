import { Typography } from "@mui/material";
import { FC } from "react";
import { DocListHeader } from "./DocListHeader";
import { List } from "./List";

const DUMMY_SUMMARY_DOCS = [
  { id: 1, name: "MCLOVE MG3", createdDate: "2020-06-02" },
  { id: 2, name: "MG05 MCLOVE", createdDate: "2020-06-02" },
  { id: 3, name: "MG05 10 June", createdDate: "2020-06-10" },
  { id: 4, name: "MG06 3 June", createdDate: "2020-06-03" },
];

const DUMMY_STATEMENTS = [
  { id: 1, name: "MG11 Shelagh MCLOVE retraction", createdDate: "2020-06-10" },
  { id: 2, name: "Shelagh McLove VPS mg11", createdDate: "2020-06-06" },
  { id: 3, name: "stmt BLAYNEE 2034 1 JUNE mg11", createdDate: "2020-06-01" },
  { id: 4, name: "stmt JONES 1989 1 JUNE mg11", createdDate: "2020-06-01" },
  { id: 5, name: "stmt Lucy Doyle MG11", createdDate: "2020-06-01" },
  { id: 6, name: "stmt Shelagh McLove MG11", createdDate: "2020-06-01" },
];

const DUMMY_DEFENDANT_DOCS = [
  { id: 1, name: "PRE CONS D", createdDate: "2020-06-02" },
  { id: 2, name: "M16", createdDate: "2020-06-02" },
];

const UNUSED_DOCS = [
  { id: 1, name: "MG06C", createdDate: "2020-06-06" },
  { id: 2, name: "MG06D", createdDate: "2020-06-06" },
  { id: 3, name: "MG06E", createdDate: "2020-06-06" },
  {
    id: 4,
    name: "UNUSED 1 - STORM LOG 1881 01.6.20 - EDITED 2020-11-23 MCLOVE",
    createdDate: "2020-06-01",
  },
];

export const SectionSummary: FC = () => {
  return (
    <>
      <DocListHeader>Case Summary</DocListHeader>
      <List heading="Case Summary Documents" documents={DUMMY_SUMMARY_DOCS}>
        <>
          <Typography variant="h6" component="h4">
            Proposed Charges
          </Typography>
          <ul>
            <li>
              <b>Section 39 Common assault 01/06/2020</b>
              <br />
              Contrary to the Criminal Justice Act 1988
            </li>
            <li>
              <b>Section 5 Drive OPL 01/06/2020</b>
              <br />
              Contrary to the Road Traffic Act
            </li>
          </ul>
        </>
      </List>
      <List heading="Statements" documents={DUMMY_STATEMENTS} />
      <List heading="Defendant Documents" documents={DUMMY_DEFENDANT_DOCS} />
      <List heading="Unused Materials" documents={UNUSED_DOCS} />
    </>
  );
};

import { Typography } from "@mui/material";
import { Box } from "@mui/system";
import { FC } from "react";
import { Link } from "react-router-dom";
import { Spacer } from "../../../../common/presentation/components/Spacer";
import {
  formatISODate,
  commonDateTimeFormats,
} from "../../../../common/utils/dates";
import { CaseSearchResult } from "../../domain/CaseSearchResult";

type ResultProps = {
  result: CaseSearchResult;
};

const getName = ({
  surname,
  firstNames,
  organisationName,
}: CaseSearchResult["leadDefendant"]) =>
  `${surname || ""}${surname && firstNames ? ", " : ""} ${firstNames || ""} ${
    organisationName || ""
  }`;

export const Result: FC<ResultProps> = ({ result }) => {
  return (
    <Box role="listitem" data-testid="element-result">
      <Spacer sx={{ marginTop: 2, marginBottom: 5 }}>
        <Link to={"/case/" + result.id} style={{ textDecoration: "none" }}>
          <Typography variant="h6">
            {result.id} &nbsp;&nbsp;&nbsp; {getName(result.leadDefendant)}
          </Typography>
        </Link>
        {result.offences.map((offence, index) => (
          <Typography key={index}>
            <b>
              {formatISODate(
                offence.earlyDate,
                commonDateTimeFormats.shortDate
              )}
            </b>{" "}
            {offence.shortDescription}
          </Typography>
        ))}
      </Spacer>
    </Box>
  );
};

import { Typography } from "@mui/material";
import { Box } from "@mui/system";
import { FC } from "react";
import { Spacer } from "../../../../presentation/common/Spacer";
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
    <Box role="listitem">
      <Spacer sx={{ marginTop: 5 }}>
        <Typography variant="h6">{result.id}</Typography>
        <Typography variant="subtitle1">
          {getName(result.leadDefendant)}
        </Typography>

        {result.offences.map((offence, index) => (
          <Typography key={index}>{offence.shortDescription}</Typography>
        ))}
      </Spacer>
    </Box>
  );
};

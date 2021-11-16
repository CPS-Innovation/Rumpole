import { Typography } from "@mui/material";
import { FC } from "react";
import { Spacer } from "../../../../../common/presentation/components/Spacer";

export const DocListHeader: FC = ({ children }) => {
  return (
    <>
      <Spacer sx={{ height: 10 }} />
      <Typography variant="h4" component="h2" sx={{ marginLeft: "15px" }}>
        {children}
      </Typography>
    </>
  );
};

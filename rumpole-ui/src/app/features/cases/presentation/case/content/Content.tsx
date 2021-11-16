import { Paper, Typography } from "@mui/material";
import { FC } from "react";

export const Content: FC = () => {
  return (
    <Paper elevation={2} sx={{ padding: "15px", height: "100%" }}>
      <Typography
        variant="h5"
        component="h2"
        sx={{
          borderBottomWidth: 1,
          borderBottomStyle: "solid",
          borderBottomColor: "grey.800",
        }}
      >
        12AB1212121 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; MCLOVE, &nbsp;&nbsp; Eoin
      </Typography>
    </Paper>
  );
};

import { Typography } from "@mui/material";
import { FC } from "react";
import { useAppTheme } from "../../../../common/theme/theme";

export const Header: FC = ({ children }) => {
  const {
    palette: { grey },
  } = useAppTheme();

  return (
    <Typography
      variant="h5"
      sx={{ borderBottom: `2px solid ${grey[400]}`, display: "flex" }}
    >
      {children}
    </Typography>
  );
};

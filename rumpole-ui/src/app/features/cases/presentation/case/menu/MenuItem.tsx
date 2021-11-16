import { SvgIconComponent } from "@mui/icons-material";
import { Box, Typography, useTheme } from "@mui/material";
import { FC } from "react";
import { generatePath, useRouteMatch } from "react-router";
import { Link } from "react-router-dom";

const grey = "grey.200";

type MenuItemProps = {
  label: string;
  Icon: SvgIconComponent;
  section: string | undefined;
};

export const MenuItem: FC<MenuItemProps> = ({ label, Icon, section }) => {
  const theme = useTheme();
  const { path, params } = useRouteMatch<{ section: string | undefined }>();
  const nextPath = generatePath(path, {
    ...params,
    section,
  });

  return (
    <Box
      sx={{
        height: 130,
        borderBottomWidth: 3,
        borderBottomStyle: "solid",
        borderBottomColor: grey,

        backgroundColor: section === params.section ? grey : "none",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
      }}
    >
      <Link
        to={nextPath}
        style={{
          textDecoration: "none",
          color: theme.palette.primary.main,
          textAlign: "center",
        }}
      >
        <Icon sx={{ fontSize: "3.5rem", color: "primary.main" }} />

        <Typography variant="body1">{label}</Typography>
      </Link>
    </Box>
  );
};

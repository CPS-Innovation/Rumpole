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
  const { path, params } = useRouteMatch<{ section: string | undefined }>();
  const theme = useTheme();

  const nextPath = generatePath(path, {
    ...params,
    section,
  });

  return (
    <Box
      sx={{
        height: 150,
        textAlign: "center",
        borderBottomWidth: 3,
        borderBottomStyle: "solid",
        borderBottomColor: grey,
        paddingTop: "15px",
        backgroundColor: section === params.section ? grey : "none",
      }}
    >
      <Link
        to={nextPath}
        style={{ textDecoration: "none", color: theme.palette.primary.main }}
      >
        <Icon sx={{ fontSize: "4rem", color: "primary.main" }} />
        <br />
        <Typography variant="body1">{label}</Typography>
      </Link>
    </Box>
  );
};
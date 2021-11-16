import { Box } from "@mui/material";
import MuiAppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import { FC } from "react";
import { Link } from "react-router-dom";
import { useUserDetails } from "../../../auth";
import { BUILD_NUMBER } from "../../../config";
import { Spacer } from "../components/Spacer";
import { Logo } from "./logo/Logo";

type AppBarProps = {
  height: number;
};

const AppBar: FC<AppBarProps> = ({ height }) => {
  const { name } = useUserDetails();
  return (
    <MuiAppBar
      position="static"
      sx={{
        display: "flex",
      }}
    >
      <Toolbar
        sx={{
          height: height - 10,
          bgcolor: "primary.main",
          borderBottom: 10,
          borderColor: "secondary.main",
        }}
      >
        <Logo height={55} />

        <Spacer sx={{ width: 40 }}></Spacer>
        <Box sx={{ flexGrow: 1 }}>
          <Link to="/" style={{ textDecoration: "none", color: "inherit" }}>
            <Typography variant="h4" component="h1">
              Rumpole
            </Typography>
          </Link>
        </Box>

        <Box sx={{ alignSelf: "flex-end" }}>
          <Typography
            aria-hidden="true"
            sx={{ color: "primary.main", textAlign: "right" }}
          >
            v: {BUILD_NUMBER}
          </Typography>

          <Typography variant="body1">{name}</Typography>
        </Box>
      </Toolbar>
    </MuiAppBar>
  );
};

export default AppBar;

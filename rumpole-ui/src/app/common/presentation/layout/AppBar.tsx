import MuiAppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import { Link } from "react-router-dom";
import { useUserDetails } from "../../../auth/useUserDetails";
import { Spacer } from "../components/Spacer";
import { Logo } from "./logo/Logo";

const AppBar: React.FC = () => {
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
          height: 90,
          bgcolor: "primary.main",
          borderBottom: 10,
          borderColor: "secondary.main",
        }}
      >
        <Spacer sx={{ marginRight: 4 }}>
          <Logo height={55} />
        </Spacer>

        <Link
          to="/"
          style={{ textDecoration: "none", color: "inherit", flexGrow: 1 }}
        >
          <Typography variant="h4" component="h1" sx={{ flexGrow: 1 }}>
            Rumpole
          </Typography>
        </Link>
        <Typography sx={{ alignSelf: "flex-end" }}>{name}</Typography>
      </Toolbar>
    </MuiAppBar>
  );
};

export default AppBar;

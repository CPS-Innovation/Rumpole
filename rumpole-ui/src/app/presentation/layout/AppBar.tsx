import MuiAppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import { Spacer } from "../common/Spacer";
import { Logo } from "./logo/Logo";

const AppBar: React.FC = () => {
  return (
    <MuiAppBar position="static" sx={{}}>
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

        <Typography variant="h4" component="h1" sx={{ flexGrow: 1 }}>
          Rumpole
        </Typography>
      </Toolbar>
    </MuiAppBar>
  );
};

export default AppBar;

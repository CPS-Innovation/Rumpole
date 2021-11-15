import { Box } from "@mui/system";
import AppBar from "./AppBar";

const APP_BAR_HEIGHT = 120;

export const Layout: React.FC = ({ children }) => {
  return (
    <Box sx={{ height: "100vh" }}>
      <AppBar height={APP_BAR_HEIGHT} />
      <Box sx={{ height: `calc(100vh - ${APP_BAR_HEIGHT}px)` }}>{children}</Box>
    </Box>
  );
};

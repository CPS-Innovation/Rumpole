import { createTheme, useTheme } from "@mui/material";

export const theme = createTheme({
  palette: {
    primary: { main: "#005daa" },
    secondary: { main: "#009ca6" },
  },
});

export const useAppTheme = () => useTheme<typeof theme>();

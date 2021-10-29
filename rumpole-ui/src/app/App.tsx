import CssBaseline from "@mui/material/CssBaseline";
import React from "react";
import { Layout } from "./layout/Layout";

// font suggested by material ui
import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";
import { ThemeProvider } from "@mui/material";
import { theme } from "./theme/theme";

export const App: React.FC = () => {
  return (
    <>
      <CssBaseline />
      <ThemeProvider theme={theme}>
        <Layout />
      </ThemeProvider>
    </>
  );
};

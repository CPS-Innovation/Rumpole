import CssBaseline from "@mui/material/CssBaseline";
import { FC } from "react";
import { Layout } from "./layout/Layout";
import { ThemeProvider } from "@mui/material";
import { theme } from "./theme/theme";
import { BrowserRouter as Router } from "react-router-dom";
import { Routes } from "./Routes";
import "./theme/font";

export const App: FC = () => {
  return (
    <>
      <CssBaseline />
      <ThemeProvider theme={theme}>
        <Router>
          <Layout>
            <Routes />
          </Layout>
        </Router>
      </ThemeProvider>
    </>
  );
};

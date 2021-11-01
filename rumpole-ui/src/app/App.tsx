import CssBaseline from "@mui/material/CssBaseline";
import { FC } from "react";
import { Layout } from "./presentation/layout/Layout";
import { ThemeProvider } from "@mui/material";
import { theme } from "./theme/theme";
import { BrowserRouter as Router } from "react-router-dom";
import { Routes } from "./Routes";
import { Provider } from "react-redux";
import { store } from "./redux/store";

require("./theme/font");

export const App: FC = () => {
  return (
    <>
      <CssBaseline />
      <ThemeProvider theme={theme}>
        <Provider store={store}>
          <Router>
            <Layout>
              <Routes />
            </Layout>
          </Router>
        </Provider>
      </ThemeProvider>
    </>
  );
};

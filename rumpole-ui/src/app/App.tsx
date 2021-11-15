import CssBaseline from "@mui/material/CssBaseline";
import { FC } from "react";
import { Layout } from "./common/presentation/layout/Layout";
import { ThemeProvider } from "@mui/material";
import { theme } from "./common/theme/theme";
import { BrowserRouter as Router } from "react-router-dom";
import { Routes } from "./Routes";
import { Provider } from "react-redux";
import { store } from "./common/redux/store";
import { Auth } from "./auth/Auth";

require("./common/theme/font");

export const App: FC = () => {
  return (
    <>
      <CssBaseline />
      <Auth>
        {(userDetails) => (
          <ThemeProvider theme={theme}>
            <Provider store={store}>
              <Router>
                <Layout>
                  <Routes />
                </Layout>
              </Router>
            </Provider>
          </ThemeProvider>
        )}
      </Auth>
    </>
  );
};

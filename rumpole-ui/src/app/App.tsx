import { FC } from "react";
import { Layout } from "./common/presentation/layout/Layout";

import { BrowserRouter as Router } from "react-router-dom";
import { Routes } from "./Routes";
import { Provider } from "react-redux";
import { store } from "./common/redux/store";
import { Auth } from "./auth";

export const App: FC = () => {
  return (
    <>
      <Auth>
        <Provider store={store}>
          <Router>
            <Layout>
              <Routes />
            </Layout>
          </Router>
        </Provider>
      </Auth>
    </>
  );
};

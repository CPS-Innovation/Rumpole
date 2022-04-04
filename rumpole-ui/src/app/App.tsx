import { FC } from "react";
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
            <Routes />
          </Router>
        </Provider>
      </Auth>
    </>
  );
};

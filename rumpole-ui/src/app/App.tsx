import { FC } from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { Routes } from "./Routes";
import { Auth } from "./auth";

export const App: FC = () => {
  return (
    <>
      <Auth>
        <Router>
          <Routes />
        </Router>
      </Auth>
    </>
  );
};

import { FC } from "react";
import { Redirect, Route, Switch } from "react-router-dom";

import { Page as UrnSearchPage } from "./features/cases/presentation/urn-search/Page";

export const Routes: FC = () => (
  <Switch>
    <Route exact path={"/search/:urn?"}>
      <UrnSearchPage />
    </Route>
    <Route>
      <Redirect to={"/search/"} />
    </Route>
  </Switch>
);

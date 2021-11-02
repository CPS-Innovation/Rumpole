import { FC } from "react";
import { Redirect, Route, Switch } from "react-router-dom";

import {
  Page as UrnSearchPage,
  path as urnSearchPath,
} from "./features/cases/presentation/search/Page";

export const Routes: FC = () => (
  <Switch>
    <Route exact path={urnSearchPath}>
      <UrnSearchPage />
    </Route>
    <Route>
      <Redirect to={"/search/"} />
    </Route>
  </Switch>
);

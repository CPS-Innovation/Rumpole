import { FC } from "react";
import { Redirect, Route, Switch } from "react-router-dom";
import { UrnSearch } from "./pages/urnSearch/UrnSearch";

export const Routes: FC = () => (
  <Switch>
    <Route exact path={"/"}>
      <UrnSearch />
    </Route>
    <Route>
      <Redirect to={"/"} />
    </Route>
  </Switch>
);

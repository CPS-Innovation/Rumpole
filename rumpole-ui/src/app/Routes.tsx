import { FC } from "react";
import { Redirect, Route, Switch } from "react-router-dom";

import {
  Page as UrnSearchPage,
  path as urnSearchPath,
} from "./features/cases/presentation/search/Page";

import {
  CasePageLayout as CasePage,
  path as casePath,
} from "./features/cases/presentation/case/CasePageLayout";
export const Routes: FC = () => (
  <Switch>
    <Route exact path={urnSearchPath}>
      <UrnSearchPage />
    </Route>
    <Route path={casePath}>
      <CasePage />
    </Route>
    <Route>
      <Redirect to={urnSearchPath} />
    </Route>
  </Switch>
);

import { FC } from "react";
import { Redirect, Route, Switch } from "react-router-dom";

import {
  Page as SearchStartPage,
  path as searchStartPath,
} from "./features/cases/presentation/search-start";

import {
  CasePageLayout as CasePage,
  path as casePath,
} from "./features/cases/presentation/case/CasePageLayout";

export const Routes: FC = () => (
  <Switch>
    <Route exact path={searchStartPath}>
      <SearchStartPage />
    </Route>
    <Route path={casePath}>
      <CasePage />
    </Route>
    <Route>
      <Redirect to={searchStartPath} />
    </Route>
  </Switch>
);

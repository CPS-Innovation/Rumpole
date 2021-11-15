import { Box } from "@mui/material";
import { FC } from "react";
import { generatePath, Route, Switch, useRouteMatch } from "react-router";
import { DocumentListSummary } from "./DocumentListSummary";

export const DocumentList: FC = () => {
  const { params, path } = useRouteMatch();
  const rootPath = generatePath(path, { ...params, section: undefined });

  console.log(rootPath);
  return (
    <Box style={{ overflowY: "scroll" }}>
      <Switch>
        <Route path={`${rootPath}`} component={DocumentListSummary} exact />
        <Route path={`${rootPath}/exhibits`} render={() => <>bbbbbbbbbb</>} />
        <Route path={`${rootPath}/redaction`} render={() => <>cccccccccc</>} />
        <Route path={`${rootPath}/all`} render={() => <>ddddddddddd</>} />
      </Switch>
    </Box>
  );
};

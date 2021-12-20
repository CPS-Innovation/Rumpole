import { ArrowCircleUp } from "@mui/icons-material";
import { Box, IconButton } from "@mui/material";
import { FC } from "react";
import { generatePath, Route, Switch, useRouteMatch } from "react-router";
import { SectionAll } from "./SectionAll";
import { SectionExhibits } from "./SectionExhibits";
import { SectionRedaction } from "./SectionRedaction";
import { SectionSummary } from "./SectionSummary";

type ListProps = {
  collapse: () => void;
};

export const DocList: FC<ListProps> = ({ collapse }) => {
  const { params, path } = useRouteMatch();
  const rootPath = generatePath(path, { ...params, section: undefined });

  return (
    <Box
      sx={{
        overflowY: "scroll",
        minHeight: "100%",
        padding: "10px",
        position: "relative",
      }}
    >
      <IconButton
        color="primary"
        aria-label="Hide document summary"
        component="span"
        sx={{
          top: 15,
          right: 15,
          position: "absolute",
          transform: "rotate(-90deg)",
        }}
        onClick={collapse}
      >
        <ArrowCircleUp fontSize="large" />
      </IconButton>
      <Switch>
        <Route path={`${rootPath}`} component={SectionSummary} exact />
        <Route path={`${rootPath}/exhibits`} component={SectionExhibits} />
        <Route path={`${rootPath}/redaction`} component={SectionRedaction} />
        <Route path={`${rootPath}/all`} component={SectionAll} />
      </Switch>
    </Box>
  );
};

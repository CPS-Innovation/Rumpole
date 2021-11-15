import { Box, Button } from "@mui/material";
import { FC, useState } from "react";
import { Spacer } from "../../../../common/presentation/components/Spacer";
import { Content } from "./Content";
import { Menu } from "./Menu";
import { DocumentList } from "./DocumentList";

const TOP_SPACER_HEIGHT = 2;
const MENU_WIDTH = 150;
const SUMMARY_EXPANDED_WIDTH = 500;
const SUMMARY_COLLAPSED_WIDTH = 0;

export const path = "/case/:id/:section?";

export const CasePageLayout: FC = () => {
  const [isExpanded, setIsExpanded] = useState<boolean>(true);
  return (
    <>
      <Spacer sx={{ height: TOP_SPACER_HEIGHT }} />
      <Box
        sx={{ height: `calc(100% - ${TOP_SPACER_HEIGHT}px)`, display: "flex" }}
      >
        <Box
          sx={{
            minHeight: "100%",
            width: MENU_WIDTH,
            borderRightWidth: 3,
            borderRightStyle: "solid",
            borderRightColor: "grey.300",
          }}
        >
          <Menu />
        </Box>

        <Box
          sx={{
            width: isExpanded
              ? SUMMARY_EXPANDED_WIDTH
              : SUMMARY_COLLAPSED_WIDTH,
            borderRightWidth: isExpanded ? 3 : 0,
            borderRightStyle: "solid",
            borderRightColor: "grey.300",
            transition: "width 0.15s",
            overflowX: "hidden",
          }}
        >
          <DocumentList />
        </Box>

        <Box sx={{ flexGrow: 1 }}>
          <Button onClick={() => setIsExpanded(!isExpanded)}>toggle</Button>
          <Content />
        </Box>
      </Box>
    </>
  );
};

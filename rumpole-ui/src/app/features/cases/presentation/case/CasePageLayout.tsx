import { Box } from "@mui/material";
import { FC, useEffect, useState } from "react";
import { Spacer } from "../../../../common/presentation/components/Spacer";
import { Content } from "./content/Content";
import { Menu } from "./menu/Menu";
import { DocList } from "./doclist/DocList";
import { useLocation } from "react-router";

const TOP_SPACER_HEIGHT = 2;
const MENU_WIDTH = 130;
const SUMMARY_EXPANDED_WIDTH = 400;
const SUMMARY_COLLAPSED_WIDTH = 0;
const BORDER_WIDTH = 3;
const grey = "grey.200";

export const path = "/case/:id/:section?";

export const CasePageLayout: FC = () => {
  const [isSummaryExpanded, setIsSummaryExpanded] = useState<boolean>(true);

  const { key } = useLocation();

  useEffect(() => {
    // if key changes (a navigation or refresh of the current route)
    //  open the list again as this is currently the only way to open a closed list.
    //  todo: is this a good way to do it?
    setIsSummaryExpanded(true);
  }, [key]);

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
            borderRightWidth: BORDER_WIDTH,
            borderRightStyle: "solid",
            borderRightColor: grey,
          }}
        >
          <Menu />
        </Box>

        <Box
          sx={{
            width: isSummaryExpanded
              ? SUMMARY_EXPANDED_WIDTH
              : SUMMARY_COLLAPSED_WIDTH,

            transition: "width 0.15s",
            overflowX: "hidden",
            backgroundColor: grey,
          }}
        >
          <Box
            sx={{
              opacity: isSummaryExpanded ? 100 : 0,
              transition: "opacity 0.1s ",
            }}
          >
            <DocList collapse={() => setIsSummaryExpanded(false)} />
          </Box>
        </Box>

        <Box sx={{ flexGrow: 1, padding: "15px" }}>
          <Content />
        </Box>
      </Box>
    </>
  );
};

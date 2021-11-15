import {
  AdminPanelSettings,
  Description,
  FormatListBulleted,
  PhotoCamera,
} from "@mui/icons-material";
import { Box } from "@mui/system";
import { FC } from "react";
import { MenuItem } from "./MenuItem";

export const Menu: FC = () => {
  return (
    <Box sx={{ display: "flex", flexFlow: "column" }}>
      <MenuItem label="URN Summary" Icon={Description} section={undefined} />
      <MenuItem label="Exhibits" Icon={PhotoCamera} section="exhibits" />
      <MenuItem
        label="Redaction"
        Icon={AdminPanelSettings}
        section="redaction"
      />
      <MenuItem
        label="All Documents on URN"
        Icon={FormatListBulleted}
        section="all"
      />
    </Box>
  );
};

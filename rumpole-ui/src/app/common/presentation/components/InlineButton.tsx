import { Button, ButtonProps } from "@mui/material";
import { FC } from "react";

export const InlineButton: FC<ButtonProps> = ({ children, ...props }) => (
  <Button
    {...props}
    sx={{
      display: "inline-block",
      padding: 0,
      minHeight: 0,
      minWidth: 0,
    }}
  >
    {children}
  </Button>
);

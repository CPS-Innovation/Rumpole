import { MsalProvider } from "@azure/msal-react";
import { Backdrop } from "@mui/material";
import { FC } from "react";
import { msalInstance } from "./msalInstance";
import { useHandleLogin } from "./useHandleLogin";

export const MsalAuth: FC = ({ children }) => {
  const loggedIn = useHandleLogin();

  return loggedIn ? (
    <MsalProvider instance={msalInstance}>{children}</MsalProvider>
  ) : (
    <Backdrop open></Backdrop>
  );
};

import { MsalProvider } from "@azure/msal-react";
import { Backdrop } from "@mui/material";
import { FC, ReactNode } from "react";
import { UserDetails } from "../UserDetails";

import { useHandleLogin } from "./useHandleLogin";
import { PublicClientApplication } from "@azure/msal-browser";
import { CLIENT_ID, TENANT_ID } from "../../config";

const msalInstance = new PublicClientApplication({
  auth: {
    clientId: CLIENT_ID,
    authority: `https://login.microsoftonline.com/${TENANT_ID}`,
    redirectUri: "/",
    postLogoutRedirectUri: "/",
  },
});

export const MsalAuth: FC<{
  children: (userDetails: UserDetails) => ReactNode;
}> = ({ children }) => {
  const userDetails = useHandleLogin(msalInstance);

  return userDetails ? (
    <MsalProvider instance={msalInstance}>{children(userDetails)}</MsalProvider>
  ) : (
    <Backdrop open></Backdrop>
  );
};

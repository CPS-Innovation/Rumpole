import { FC, ReactNode } from "react";
import { MockAuth } from "./mock/MockAuth";
import { MsalAuth } from "./msal/MsalAuth";
import { UserDetails } from "./UserDetails";

export const Auth: FC<{
  children: (adUserDetails: UserDetails) => ReactNode;
}> = ({ children }) =>
  process.env.REACT_APP_MOCK_AUTH === "true" ? (
    <MockAuth>{(userDetails) => children(userDetails)}</MockAuth>
  ) : (
    <MsalAuth>{(userDetails) => children(userDetails)}</MsalAuth>
  );

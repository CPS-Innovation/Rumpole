import { FC, ReactNode } from "react";
import { UserDetails } from "../UserDetails";

export const MockAuth: FC<{
  children: (adUserDetails: UserDetails) => ReactNode;
}> = ({ children }) => {
  const userDetails: UserDetails = {
    name: "Dev User",
    username: "dev_user@example.org",
    getAccessToken: () => Promise.resolve("dev_token"),
  };
  return <>{children(userDetails)}</>;
};

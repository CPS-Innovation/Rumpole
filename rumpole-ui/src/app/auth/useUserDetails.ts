import { useAccount, useMsal } from "@azure/msal-react";
import { UserDetails } from "./UserDetails";

export const useUserDetails = () => {
  const { accounts } = useMsal();
  const account = useAccount(accounts[0] || {});

  return {
    name: account?.name || "",
    username: account?.username,
  } as UserDetails;
};

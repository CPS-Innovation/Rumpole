import {
  InteractionRequiredAuthError,
  PublicClientApplication,
  SilentRequest,
} from "@azure/msal-browser";
import { useEffect, useState } from "react";
import { UserDetails } from "../UserDetails";

export const useHandleLogin = (msalInstance: PublicClientApplication) => {
  const [userDetails, setUserDetails] = useState<UserDetails>();

  useEffect(() => {
    (async () => {
      // required so that when we are coming back from a redirect, that process is complete
      //  before we do any more auth interactions (otherwise an error is thrown)
      await msalInstance.handleRedirectPromise();

      const [account] = msalInstance.getAllAccounts();

      if (!account) {
        await msalInstance.loginRedirect({
          scopes: ["User.Read"],
          prompt: "select_account",
        });
        return;
      }

      const getAccessToken = async (scopes: string[]) => {
        const [account] = msalInstance.getAllAccounts();

        type NewType = SilentRequest;

        const request = {
          scopes,
          account,
        } as NewType;

        try {
          const { accessToken } = await msalInstance.acquireTokenSilent(
            request
          );
          return accessToken;
        } catch (error) {
          if (error instanceof InteractionRequiredAuthError) {
            await msalInstance.acquireTokenRedirect(request);
          }
          return String(); // so the return type is always string
        }
      };

      setUserDetails({
        name: account.name || "Name Unknown",
        username: account.username,
        getAccessToken,
      });
    })();
  }, [msalInstance]);

  return userDetails;
};

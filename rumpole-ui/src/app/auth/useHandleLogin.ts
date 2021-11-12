import { useEffect, useState } from "react";
import { msalInstance } from "./msalInstance";

export const useHandleLogin = () => {
  const [loggedIn, setLoggedIn] = useState<boolean>(false);

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

      setLoggedIn(true);
    })();
  }, []);
  return loggedIn;
};

import { Auth as MockAuth } from "./mock/Auth";
import { Auth as MsalAuth } from "./msal/Auth";
import { useUserDetails as useMockUserDetails } from "./mock/useUserDetails";
import { useUserDetails as useMsalUserDetails } from "./msal/useUserDetails";
import { getAccessToken as mockGetAccessToken } from "./mock/getAccessToken";
import { getAccessToken as msalGetAccessToken } from "./msal/getAccessToken";

const IS_MOCK = process.env.REACT_APP_MOCK_AUTH === "true";

export const Auth = IS_MOCK ? MockAuth : MsalAuth;

export const useUserDetails = IS_MOCK ? useMockUserDetails : useMsalUserDetails;

export const getAccessToken = IS_MOCK ? mockGetAccessToken : msalGetAccessToken;

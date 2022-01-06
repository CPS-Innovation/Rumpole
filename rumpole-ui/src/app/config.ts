export const GATEWAY_BASE_URL = process.env.REACT_APP_GATEWAY_BASE_URL!;
export const GATEWAY_SCOPE = process.env.REACT_APP_GATEWAY_SCOPE!;
export const CLIENT_ID = process.env.REACT_APP_CLIENT_ID!;
export const TENANT_ID = process.env.REACT_APP_TENANT_ID!;
export const BUILD_NUMBER = process.env.REACT_APP_BUILD_NUMBER || "development";

console.log(
  JSON.stringify({
    GATEWAY_BASE_URL,
    GATEWAY_SCOPE,
    CLIENT_ID,
    TENANT_ID,
    BUILD_NUMBER,
  })
);

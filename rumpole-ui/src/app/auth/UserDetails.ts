export type UserDetails = {
  name: string;
  username: string;
  getAccessToken: (scopes: string[]) => Promise<string>;
};

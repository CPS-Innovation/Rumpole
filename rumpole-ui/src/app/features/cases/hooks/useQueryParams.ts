import { useHistory, useLocation } from "react-router-dom";
import { parse, stringify } from "qs";
import { QueryParams } from "../types/QueryParams";

export const useQueryParams = <T>() => {
  const { search } = useLocation();
  const params = parse(search, { ignoreQueryPrefix: true }) as unknown as T;

  const { push } = useHistory();
  const setParams = (params: QueryParams) => {
    const path = stringify(params, { addQueryPrefix: true });
    push(path);
  };

  return {
    setParams,
    params,
  };
};

import { useHistory, useLocation } from "react-router-dom";
import { parse, stringify } from "qs";
import { path } from "../../features/cases/presentation/case-search-results";

export type QueryParamsState<T> = {
  setParams: (params: Partial<T>) => void;
  params: Partial<T>;
  search: string;
};

export const useQueryParamsState = <T>(): QueryParamsState<T> => {
  const { search } = useLocation();
  const { push } = useHistory();

  const params = parse(search, {
    ignoreQueryPrefix: true,
    comma: true,
  }) as unknown as T;

  const setParams = (params: Partial<T>) => {
    const queryString = stringify(params, {
      addQueryPrefix: true,
      encode: false,
    });
    push(`${path}${queryString}`); //todo: pass path in
  };

  return {
    setParams,
    params,
    search,
  };
};

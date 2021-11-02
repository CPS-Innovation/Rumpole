import { useHistory, useLocation } from "react-router-dom";
import { parse, stringify } from "qs";

export const useQueryParams = <T>() => {
  const { search } = useLocation();
  const params = parse(search, {
    ignoreQueryPrefix: true,
    comma: true,
  }) as unknown as T;

  const { push } = useHistory();

  const setParams = (params: Partial<T>) => {
    // todo: better, or comment
    const weaklyTypedParams = { ...params } as { [key: string]: any };
    const paramsToStringify = Object.keys(weaklyTypedParams).reduce(
      (acc, curr) => {
        const value = weaklyTypedParams[curr];
        acc[curr] = Array.isArray(value) && value.length === 0 ? "" : value;
        return acc;
      },
      {} as { [key: string]: any }
    );

    const path = stringify(paramsToStringify, {
      addQueryPrefix: true,
      encode: false,
      arrayFormat: "comma",
    });
    push(path);
  };

  return {
    setParams,
    params,
  };
};

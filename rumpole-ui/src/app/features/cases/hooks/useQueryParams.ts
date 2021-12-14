import { useHistory, useLocation } from "react-router-dom";
import { parse, stringify } from "qs";

export type QueryParamsState<T> = {
  setParams: (params: Partial<T>) => void;
  params: Partial<T>;
};

export const useQueryParams = <T>(): QueryParamsState<T> => {
  const { search } = useLocation();
  const { push } = useHistory();

  const params = parse(search, {
    ignoreQueryPrefix: true,
    comma: true,
  }) as unknown as T;

  const setParams = (params: Partial<T>) => {
    const paramsToStringify = convertEmptyArrayToString(params);

    const path = stringify(paramsToStringify, {
      addQueryPrefix: true,
      encode: false,
      // use comma form to allow empty arrays to be indicated in the querystring
      //  note: if param "a" is missing, that means all values for "a" should be shown
      arrayFormat: "comma",
    });
    push(path);
  };

  return {
    setParams,
    params,
  };
};

// for an array type parameter, if the array is empty we need qs to retain
//  the empty query string parameter (i.e. return "?a=" if a =[], rather than removing the "a" param)
//  so we convert the empty array to empty string
const convertEmptyArrayToString = (object: any) =>
  Object.keys(object).reduce((acc, curr) => {
    const value = object[curr];
    acc[curr] = Array.isArray(value) && value.length === 0 ? "" : value;
    return acc;
  }, {} as { [key: string]: any });

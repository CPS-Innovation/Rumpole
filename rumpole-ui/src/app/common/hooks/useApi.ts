import { useEffect, useState } from "react";

type Result<T> =
  | {
      data: T;
      status: "succeeded";
    }
  | {
      error: any;
      status: "failed";
    }
  | {
      status: "loading";
    };

type AllowedPrimitive = string | number | boolean | null | undefined;

type UseApiParams = <T extends (...args: any[]) => Promise<any>>(
  del: T,
  param0?: Parameters<T>[0] extends AllowedPrimitive ? AllowedPrimitive : never,
  param1?: Parameters<T>[1] extends AllowedPrimitive ? AllowedPrimitive : never,
  param2?: Parameters<T>[2] extends AllowedPrimitive ? AllowedPrimitive : never,
  param3?: Parameters<T>[3] extends AllowedPrimitive ? AllowedPrimitive : never,
  param4?: Parameters<T>[4] extends AllowedPrimitive ? AllowedPrimitive : never
) => Result<Awaited<ReturnType<typeof del>>>;

/*
  If there is an api method `getFoo(id: number, name: string) => Promise<Model>` then `useApi` is called thus:
    `const state = useApi(getFoo, 1, "bar")`

  The `UseApiParams` type ensures that the second (third.. etc) paramters passed to useApi are
  strongly-typed to the argument types of the function passed as the first param e.g. `getFoo`.

  This approach is borrrowed from redux-sagas and avoids the use of anonymous lambdas being passed e.g.

    `const state = useApi(() => getFoo(1, "bar"))`

  meaning that on the inside of `useApi` the function is always seen as being different on every execution
  leading to constant refiring of the `useEffect`.  The use of `AllowedPrimitive` also helps to prevent
  object (compared by reference) being passed and be seen to differ every execution also, safer to just pass 
  primitives.
*/
export const useApi: UseApiParams = (del, p0, p1, p2, p3, p4) => {
  const [result, setResult] = useState<ReturnType<UseApiParams>>({
    status: "loading",
  });

  useEffect(() => {
    setResult({ status: "loading" });

    del
      .apply(del, [p0, p1, p2, p3, p4])
      .then((data) =>
        setResult({
          status: "succeeded",
          data,
        })
      )
      .catch((error) =>
        setResult({
          status: "failed",
          error,
        })
      );
  }, [del, p0, p1, p2, p3, p4]);

  return result;
};

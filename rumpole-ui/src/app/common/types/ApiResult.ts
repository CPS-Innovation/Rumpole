import { DelayedResult } from "./DelayedResult";

export type ApiResult<T> =
  // not particularly graceful but only way I found to "inherit" fromn another union type.
  | Extract<DelayedResult<T>, { status: "loading" }>
  | Extract<DelayedResult<T>, { status: "succeeded" }>
  | {
      error: any;
      status: "failed";
      httpStatusCode: number | undefined;
    };

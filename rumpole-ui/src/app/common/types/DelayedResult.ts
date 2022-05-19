export type DelayedResult<T> =
  | {
      status: "succeeded";
      data: T;
    }
  | {
      status: "loading";
    };

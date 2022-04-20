import "@testing-library/jest-dom";
import "@testing-library/jest-dom/extend-expect";
import { renderHook } from "@testing-library/react-hooks";
import { useApi } from "./useApi";

describe("useApi", () => {
  it("can initiate a call, set status to loading, then return a successful result", async () => {
    const mockApiCall = (id: string) =>
      new Promise((resolve) => resolve({ id }));

    const { result, waitForNextUpdate } = renderHook(() =>
      useApi(mockApiCall, "1")
    );

    expect(result.current).toEqual({ status: "loading" });
    await waitForNextUpdate();

    expect(result.current).toEqual({ status: "succeeded", data: { id: "1" } });
  });

  it("can initiate a call, set status to loading, then return an error result", async () => {
    const apiError = new Error();

    const mockApiCall = (id: string) =>
      new Promise((_, reject) => reject(apiError));

    const { result, waitForNextUpdate } = renderHook(() =>
      useApi(mockApiCall, "1")
    );

    expect(result.current).toEqual({ status: "loading" });
    await waitForNextUpdate();

    expect(result.current).toEqual({ status: "failed", error: apiError });
  });

  it("can initiate a call with multiple parameters", async () => {
    const mockApiCall = (id: string, otherValue: number, thirdValue: string) =>
      new Promise((resolve) => resolve({ id, otherValue, thirdValue }));

    const { result, waitForNextUpdate } = renderHook(() =>
      useApi(mockApiCall, "1", 2, "3")
    );

    await waitForNextUpdate();

    expect(result.current).toEqual({
      status: "succeeded",
      data: { id: "1", otherValue: 2, thirdValue: "3" },
    });
  });
});

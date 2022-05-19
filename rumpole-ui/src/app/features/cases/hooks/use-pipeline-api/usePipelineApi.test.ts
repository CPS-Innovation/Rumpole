import { usePipelineApi } from "./usePipelineApi";
import * as polling from "./initiateAndPoll";
import { PipelineResults } from "../../domain/PipelineResults";
import { ApiResult } from "../../../../common/types/ApiResult";
import { renderHook } from "@testing-library/react-hooks";
describe("usePipelineApi", () => {
  it("can return results", async () => {
    const expectedResults = {} as ApiResult<PipelineResults>;

    jest
      .spyOn(polling, "initiateAndPoll")
      .mockImplementation((caseId, pollingDelay, del) => {
        new Promise((resolve) => setTimeout(resolve, 50)).then(() =>
          del(expectedResults)
        );
        return () => {};
      });

    const { result, waitForNextUpdate } = renderHook(() => usePipelineApi("1"));

    expect(result.current).toEqual({ status: "loading" });

    await waitForNextUpdate();

    expect(result.current).toEqual(expectedResults);
  });
});

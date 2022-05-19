import { ApiResult } from "../../../../common/types/ApiResult";
import { PipelineResults } from "../../domain/PipelineResults";
import { initiateAndPoll } from "./initiateAndPoll";
import * as api from "../../api/gateway-api";
import { waitFor } from "@testing-library/react";
import { ApiError } from "../../../../common/errors/ApiError";

const POLLING_INTERVAL_MS = 75;

const ensureHasStoppedPollingHelper = async (
  quitFn: () => void,
  spy: jest.SpyInstance
) => {
  quitFn();
  const callsMadeSoFar = spy.mock.calls.length;
  await new Promise((resolve) => setTimeout(resolve, POLLING_INTERVAL_MS * 3));
  expect(spy.mock.calls.length).toEqual(callsMadeSoFar);
};

describe("initiateAndPoll", () => {
  it("can return failed and stop polling if initiate errors", async () => {
    const expectedError = new ApiError("", "", { status: 100, statusText: "" });
    const spy = jest
      .spyOn(api, "initiatePipeline")
      .mockImplementation((caseId) => Promise.reject(expectedError));

    let results: ApiResult<PipelineResults>;
    const quitFn = initiateAndPoll(
      "1",
      POLLING_INTERVAL_MS,
      (res) => (results = res)
    );

    await waitFor(() =>
      expect(results).toEqual({
        status: "failed",
        error: expectedError,
        httpStatusCode: 100,
      } as ApiResult<PipelineResults>)
    );

    ensureHasStoppedPollingHelper(quitFn, spy);
  });

  it("can return failed and stop polling if getPipelinePdfResults errors", async () => {
    jest
      .spyOn(api, "initiatePipeline")
      .mockImplementation((caseId) => Promise.resolve("foo"));

    const expectedError = new ApiError("", "", { status: 100, statusText: "" });
    const spy = jest
      .spyOn(api, "getPipelinePdfResults")
      .mockImplementation((caseId) => Promise.reject(expectedError));

    let results: ApiResult<PipelineResults>;
    const quitFn = initiateAndPoll(
      "1",
      POLLING_INTERVAL_MS,
      (res) => (results = res)
    );

    await waitFor(() =>
      expect(results).toEqual({
        status: "failed",
        error: expectedError,
        httpStatusCode: 100,
      } as ApiResult<PipelineResults>)
    );

    ensureHasStoppedPollingHelper(quitFn, spy);
  });

  it("can return an immediately available result", async () => {
    jest
      .spyOn(api, "initiatePipeline")
      .mockImplementation((caseId) => Promise.resolve("foo"));

    const expectedResults = {
      transationId: "",
      documents: [{ pdfBlobName: "foo" }],
    } as PipelineResults;

    const spy = jest
      .spyOn(api, "getPipelinePdfResults")
      .mockImplementation((caseId) => Promise.resolve(expectedResults));

    let results: ApiResult<PipelineResults>;
    const quitFn = initiateAndPoll(
      "1",
      POLLING_INTERVAL_MS,
      (res) => (results = res)
    );

    await waitFor(() =>
      expect(results).toEqual({ status: "succeeded", data: expectedResults })
    );

    ensureHasStoppedPollingHelper(quitFn, spy);
  });

  it("can poll to retrieve a result", async () => {
    jest
      .spyOn(api, "initiatePipeline")
      .mockImplementation((caseId) => Promise.resolve("foo"));

    const expectedInterimResults = {
      transationId: "",
      documents: [{ pdfBlobName: "foo" }, {}],
    } as PipelineResults;

    const expectedFinalResults = {
      transationId: "",
      documents: [{ pdfBlobName: "foo" }, { pdfBlobName: "bar" }],
    } as PipelineResults;

    let runIndex = 0;
    const spy = jest
      .spyOn(api, "getPipelinePdfResults")
      .mockImplementation((caseId) => {
        if (runIndex === 0) {
          runIndex += 1;
          return Promise.resolve(expectedInterimResults);
        } else {
          return Promise.resolve(expectedFinalResults);
        }
      });

    let results: ApiResult<PipelineResults>;
    const quitFn = initiateAndPoll(
      "1",
      POLLING_INTERVAL_MS,
      (res) => (results = res)
    );

    await waitFor(() =>
      expect(results).toEqual({
        status: "succeeded",
        data: expectedInterimResults,
      })
    );
    await waitFor(() =>
      expect(results).toEqual({
        status: "succeeded",
        data: expectedFinalResults,
      })
    );

    ensureHasStoppedPollingHelper(quitFn, spy);
  });
});

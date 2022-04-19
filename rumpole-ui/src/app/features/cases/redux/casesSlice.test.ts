import { AnyAction } from "redux";
import { CaseDetails } from "../domain/CaseDetails";
import reducer, { CasesState, fetchCases } from "./casesSlice";

describe("casesSlice", () => {
  test("should return the initial state", () => {
    expect(reducer(undefined, {} as AnyAction)).toEqual({
      data: { entities: {}, ids: [] },
      error: undefined,
      status: "idle",
      urn: undefined,
    } as CasesState);
  });

  test("should initiate loading and set the urn", () => {
    const prevState = {
      data: { entities: {}, ids: [] },
      error: undefined,
      status: "idle",
      urn: "foo",
    } as CasesState;

    expect(reducer(prevState, fetchCases.pending("", "bar", ""))).toEqual({
      data: { entities: {}, ids: [] },
      error: undefined,
      status: "loading",
      urn: "bar",
    });
  });

  test("should complete loading and set the urn", () => {
    const prevState = {
      data: { entities: {}, ids: [] },
      error: undefined,
      status: "loading",
      urn: "foo",
    } as CasesState;

    const expectedResult1 = { id: 1 } as CaseDetails;
    const expectedResult2 = { id: 2 } as CaseDetails;
    expect(
      reducer(
        prevState,
        fetchCases.fulfilled([expectedResult1, expectedResult2], "", "")
      )
    ).toEqual({
      data: {
        entities: { 1: expectedResult1, 2: expectedResult2 },
        ids: [1, 2],
      },
      error: undefined,
      status: "succeeded",
      urn: "foo",
    });
  });

  test("should handle error", () => {
    const prevState = {
      data: { entities: {}, ids: [] },
      error: undefined,
      status: "loading",
      urn: "foo",
    } as CasesState;

    expect(
      reducer(prevState, fetchCases.rejected(new Error("bar"), "", ""))
    ).toEqual({
      data: {
        entities: {},
        ids: [],
      },
      error: "bar",
      status: "failed",
      urn: "foo",
    });
  });
});

import {
  createSlice,
  createAsyncThunk,
  createEntityAdapter,
} from "@reduxjs/toolkit";

import { RootState } from "../../../common/redux/store";
import { searchUrn } from "../api/gatewayApi";
import { searchUrn as coreDataSearchUrn } from "../api/coreDataApi";
import { CaseSearchResult } from "../domain/CaseSearchResult";

const caseAdapter = createEntityAdapter<CaseSearchResult>();

export type CasesState = {
  urn: string | undefined;
  data: ReturnType<typeof caseAdapter.getInitialState>;
  status: "idle" | "loading" | "succeeded" | "failed";
  error: string | undefined;
};

export const fetchCases = createAsyncThunk(
  "cases/fetchCases",
  async (urn: string) => await coreDataSearchUrn(urn)
);

const initialState: CasesState = {
  urn: undefined,
  data: caseAdapter.getInitialState(),
  status: "idle",
  error: undefined,
};

export const casesSlice = createSlice({
  name: "cases",
  initialState,
  reducers: {
    clearCases: (state) => {
      Object.assign(state, initialState);
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchCases.pending, (state, action) => {
        state.urn = action.meta.arg;
        state.status = "loading";
        caseAdapter.setAll(state.data, []);
      })
      .addCase(fetchCases.fulfilled, (state, action) => {
        state.status = "succeeded";
        caseAdapter.setAll(state.data, action.payload);
      })
      .addCase(fetchCases.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message;
      });
  },
});

export const { clearCases } = casesSlice.actions;

export const { selectAll } = caseAdapter.getSelectors<RootState>(
  (state) => state.cases.data
);

export default casesSlice.reducer;

import {
  createSlice,
  createAsyncThunk,
  createEntityAdapter,
} from "@reduxjs/toolkit";
import { RootState } from "../../../redux/store";
import { searchUrn } from "../api/gatewayApt";
import { CaseSearchResult } from "../domain/CaseSearchResult";

const caseAdapter = createEntityAdapter<CaseSearchResult>();

type CasesState = {
  cases: ReturnType<typeof caseAdapter.getInitialState>;
  status: "idle" | "loading" | "succeeded" | "failed";
  error: string | undefined;
};

export const fetchCases = createAsyncThunk(
  "cases/fetchCases",
  async (urn: string) => await searchUrn(urn)
);

const initialState: CasesState = {
  cases: caseAdapter.getInitialState(),
  status: "idle",
  error: undefined,
};

export const casesSlice = createSlice({
  name: "cases",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchCases.pending, (state) => {
        state.status = "loading";
      })
      .addCase(fetchCases.fulfilled, (state, action) => {
        state.status = "succeeded";
        caseAdapter.setAll(state.cases, action.payload);
      })
      .addCase(fetchCases.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message;
      });
  },
});

export const { selectIds, selectById, selectAll } =
  caseAdapter.getSelectors<RootState>((state) => state.cases.cases);

export default casesSlice.reducer;

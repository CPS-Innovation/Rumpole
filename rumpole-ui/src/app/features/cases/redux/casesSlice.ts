import {
  createSlice,
  createAsyncThunk,
  createEntityAdapter,
} from "@reduxjs/toolkit";
import { RootState } from "../../../redux/store";
import { UrnSearchResult } from "../types/models/UrnSearchResult";

const caseAdapter = createEntityAdapter<UrnSearchResult>({
  selectId: (item) => item.caseId,
});

type CasesState = {
  cases: ReturnType<typeof caseAdapter.getInitialState>;
  status: "idle" | "loading" | "succeeded" | "failed";
  error: string | undefined;
};

export const fetchCases = createAsyncThunk(
  "cases/fetchCases",
  async (urn: string) => {
    const response = await fetch(
      `https://jsonplaceholder.typicode.com/users?urn=${urn}`
    );
    return (await response.json()) as UrnSearchResult[];
  }
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

export const { selectIds, selectById } = caseAdapter.getSelectors<RootState>(
  (state) => state.cases.cases
);

export default casesSlice.reducer;

import {
  createSlice,
  createAsyncThunk,
  createEntityAdapter,
} from "@reduxjs/toolkit";

import { RootState } from "../../../common/redux/store";
import { getCaseDetails } from "../api/gatewayApi";
import { CaseDetails } from "../domain/CaseDetails";

const caseAdapter = createEntityAdapter<CaseDetails>();

export type CaseDetailsState = {
  data: ReturnType<typeof caseAdapter.getInitialState>;
  status: "idle" | "loading" | "succeeded" | "failed";
  error: string | undefined;
};

export const fetchCase = createAsyncThunk(
  "case-details/fetchCase",
  async (caseId: string) => await getCaseDetails(caseId)
);

const initialState: CaseDetailsState = {
  data: caseAdapter.getInitialState(),
  status: "idle",
  error: undefined,
};

export const caseDetailsSlice = createSlice({
  name: "cases",
  initialState,
  reducers: {
    clearCases: (state) => {
      Object.assign(state, initialState);
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchCase.pending, (state, action) => {
        state.status = "loading";
        caseAdapter.setAll(state.data, []);
      })
      .addCase(fetchCase.fulfilled, (state, action) => {
        state.status = "succeeded";
        caseAdapter.setAll(state.data, [action.payload]);
      })
      .addCase(fetchCase.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message;
      });
  },
});

export const { clearCases: clearCase } = caseDetailsSlice.actions;

export const { selectAll } = caseAdapter.getSelectors<RootState>(
  (state) => state.caseDetails.data
);

export default caseDetailsSlice.reducer;

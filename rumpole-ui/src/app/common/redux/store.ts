import { configureStore } from "@reduxjs/toolkit";
import casesReducer from "../../features/cases/redux/casesSlice";
import caseDetailsReducer from "../../features/cases/redux/caseDetailsSlice";

export const store = configureStore({
  reducer: {
    cases: casesReducer,
    caseDetails: caseDetailsReducer,
  },
  devTools: true,
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

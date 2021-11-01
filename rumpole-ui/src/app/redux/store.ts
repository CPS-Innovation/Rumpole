import { configureStore } from "@reduxjs/toolkit";
import casesReducer from "../features/cases/redux/casesSlice";

export const store = configureStore({
  reducer: {
    cases: casesReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

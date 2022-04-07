import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "../../../common/redux/hooks";
import { clearCases, fetchCases, selectAll } from "../redux/casesSlice";

export type SearchReduxState = ReturnType<typeof useSearchReduxState>;

export const useSearchReduxState = (urn: string | undefined) => {
  const data = useAppSelector(selectAll);
  const {
    status,
    urn: reduxUrn,
    error,
  } = useAppSelector((state) => state.cases);

  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(urn ? fetchCases(urn) : clearCases());
  }, [dispatch, urn]);

  return {
    totalCount: data.length,
    data,
    reduxUrn,
    loadingStatus: urn !== reduxUrn ? "idle" : status,
    error,
  };
};

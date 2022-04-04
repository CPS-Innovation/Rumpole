import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "../../../common/redux/hooks";
import { clearCases, fetchCases, selectAll } from "../redux/casesSlice";

export type SearchDataState = ReturnType<typeof useSearchDataState>;

export const useSearchDataState = (urn: string | undefined) => {
  const data = useAppSelector(selectAll);
  const {
    status: loadingStatus,
    urn: reduxUrn,
    error,
  } = useAppSelector((state) => state.cases);

  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(urn ? fetchCases(urn) : clearCases());
  }, [dispatch, urn]);

  return {
    data,
    reduxUrn,
    loadingStatus,
    error,
  };
};

import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "../../../common/redux/hooks";
import { clearCases, fetchCases, selectAll } from "../redux/casesSlice";

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

  const loadingStatus = urn !== reduxUrn ? "idle" : status;

  return {
    totalCount: data.length,
    data,
    reduxUrn,
    loadingStatus,
    error,
  };
};

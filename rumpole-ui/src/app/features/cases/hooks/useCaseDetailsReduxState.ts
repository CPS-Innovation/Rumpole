import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "../../../common/redux/hooks";
import { clearCase, fetchCase, selectAll } from "../redux/caseDetailsSlice";

export const useCaseDetailsReduxState = (id: string | undefined) => {
  const data = useAppSelector(selectAll);
  const { status, error } = useAppSelector((state) => state.caseDetails);

  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(id ? fetchCase(id) : clearCase());
  }, [dispatch, id]);

  const caseDetails = data.find((item) => String(item.id) === id);

  const loadingStatus =
    id !== undefined && String(caseDetails?.id) !== id ? "idle" : status;

  console.log(loadingStatus);

  return {
    item: caseDetails,
    loadingStatus,
    error,
  };
};

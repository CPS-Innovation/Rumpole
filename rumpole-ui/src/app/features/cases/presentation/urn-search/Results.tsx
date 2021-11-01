import { Container } from "@mui/material";
import { FC, useEffect } from "react";
import { useAppDispatch, useAppSelector } from "../../../../redux/hooks";
import { fetchCases, selectIds, selectById } from "../../redux/casesSlice";
import { UrnFilterQueryParams } from "../../types/UrnFilterQueryParams";

type ResultsProps = {
  onFilter: (queryParams: UrnFilterQueryParams) => void;
};

export const Results: FC<ResultsProps> = ({ onFilter }) => {
  const dispatch = useAppDispatch();
  useEffect(() => {
    dispatch(fetchCases("abc"));
  }, [dispatch]);

  const caseIds = useAppSelector(selectIds);
  const exampleCase = useAppSelector((state) => selectById(state, "1"));

  return (
    <Container>
      <ul>
        {caseIds.map((id) => (
          <li key={id}>{id}</li>
        ))}
        <li>{exampleCase?.name}</li>
      </ul>
    </Container>
  );
};

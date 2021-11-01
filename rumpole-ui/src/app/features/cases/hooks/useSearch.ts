import { useHistory, generatePath, useParams } from "react-router-dom";

export const useSearch = <T>() => {
  const { push } = useHistory();
  const params = useParams<{ urn: string }>();

  const handleSearch = (urn: string) => {
    const path = generatePath("/search/:urn?", {
      urn,
    });
    push(path);
  };

  const handleFilter = (queryParams: T) => null;

  return {
    handleSearch,
    handleFilter,
    urn: params.urn,
  };
};

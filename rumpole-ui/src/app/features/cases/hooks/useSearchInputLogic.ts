import { useState, KeyboardEvent } from "react";
import { isUrnValid } from "../logic/isUrnValid";
import { CaseSearchQueryParams } from "../types/CaseSearchQueryParams";

export const useSearchInputLogic = ({
  initialUrn,
  setParams,
}: {
  initialUrn: string | undefined;
  setParams: (params: Partial<CaseSearchQueryParams>) => void;
}) => {
  const [urn, setUrn] = useState(initialUrn || "");
  const [isError, setIsError] = useState(false);

  const isValid = isUrnValid(urn);

  const handleChange = (val: string) => {
    setUrn(val.toUpperCase());
  };

  const handleSubmit = () => {
    setIsError(!isValid);
    if (isValid) {
      setParams({ urn });
    }
  };

  const handleKeyPress = (event: KeyboardEvent<HTMLInputElement>) =>
    event.key === "Enter" && handleSubmit();

  return {
    handleChange,
    handleKeyPress,
    handleSubmit,
    urn,
    isError,
  };
};

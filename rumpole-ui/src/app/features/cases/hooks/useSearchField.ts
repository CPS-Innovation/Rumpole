import { useState, KeyboardEvent } from "react";
import { isUrnValid } from "../logic/isUrnValid";
import { SearchState } from "./useSearchState";

export const useSearchField = ({
  urn: initialUrn,
  setUrnParam,
}: SearchState) => {
  const [urn, setUrn] = useState(initialUrn || "");
  const [isError, setIsError] = useState(false);

  const isValid = isUrnValid(urn);

  const handleChange = (val: string) => setUrn(val.toUpperCase());

  const handleSubmit = () => {
    setIsError(!isValid);
    if (isValid) {
      setUrnParam(urn);
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

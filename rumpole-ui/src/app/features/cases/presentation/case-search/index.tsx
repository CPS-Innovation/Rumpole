import React, { useState, KeyboardEvent } from "react";
import { useQueryParamsState } from "../../hooks/useQueryParams";
import { CaseFilterQueryParams } from "../../hooks/types/CaseFilterQueryParams";
import { useSearchDataState } from "../../hooks/useSearchDataState";
import { useSearchState } from "../../hooks/useSearchState";
import { isUrnValid } from "../../logic/isUrnValid";
import {
  Button,
  Hint,
  Input,
} from "../../../../common/presentation/components";

export const path = "/case-search";

const Page: React.FC = () => {
  const searchFilterState = useQueryParamsState<CaseFilterQueryParams>();
  const searchDataState = useSearchDataState(searchFilterState.params.urn);

  const { urn: initialUrn, setUrnParam } = useSearchState(
    searchFilterState,
    searchDataState
  );

  const [urn, setUrn] = useState(initialUrn || "");
  const isValid = isUrnValid(urn);

  const handleChange = (val: string) => setUrn(val.toUpperCase());

  const handleSubmit = () => {
    isValid && setUrnParam(urn);
  };

  const handleKeyPress = (event: KeyboardEvent<HTMLInputElement>) =>
    event.key === "Enter" && handleSubmit();

  return (
    <div className="govuk-grid-row">
      <div className="govuk-grid-column-two-thirds">
        <h1 className="govuk-heading-xl">
          Find a case
          <Hint>Search and review a CPS case in England</Hint>
        </h1>

        <div className="govuk-form-group">
          <Input
            onChange={handleChange}
            onKeyPress={handleKeyPress}
            value={urn}
            autoFocus
            data-testid="input-search-urn"
            label={{
              className: "govuk-label--s",
              children: "Search for a case URN",
            }}
          />
        </div>
        <Button
          onClick={handleSubmit}
          disabled={!isValid}
          data-testid="button-search"
        >
          Search
        </Button>
      </div>
    </div>
  );
};

export default Page;

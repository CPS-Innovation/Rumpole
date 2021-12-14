import React, { useState, KeyboardEvent } from "react";
import * as GDS from "../../../../common/presentation/components";
import { useQueryParams } from "../../hooks/useQueryParams";
import { CaseFilterQueryParams } from "../../hooks/types/CaseFilterQueryParams";
import { useSearchDataState } from "../../hooks/useSearchDataState";
import { useSearchState } from "../../hooks/useSearchState";
import { isUrnValid } from "../../logic/isUrnValid";
export const path = "/search";

export const Page: React.FC = () => {
  const searchFilterState = useQueryParams<CaseFilterQueryParams>();
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
    <>
      <>
        <div className="govuk-grid-row">
          <div className="govuk-grid-column-two-thirds">
            <h1 className="govuk-heading-xl">
              Find a case
              <GDS.Hint>Search and review a CPS case in England</GDS.Hint>
            </h1>

            <div className="govuk-form-group">
              <GDS.Label>Search for a case URN</GDS.Label>

              <GDS.Input
                onChange={handleChange}
                onKeyPress={handleKeyPress}
                value={urn}
                autoFocus
                data-testid="input-search-urn"
              />
            </div>
            <GDS.Button
              onClick={handleSubmit}
              disabled={!isValid}
              data-testid="button-search"
            >
              Search
            </GDS.Button>
          </div>
        </div>
      </>
    </>
  );
};

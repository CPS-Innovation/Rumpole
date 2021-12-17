import React, { useEffect } from "react";
import { useQueryParamsState } from "../../../../common/hooks/useQueryParamsState";
import { CaseFilterQueryParams } from "../../hooks/types/CaseFilterQueryParams";
import { useSearchDataState } from "../../hooks/useSearchDataState";
import { useSearchState } from "../../hooks/useSearchState";
import {
  Button,
  Hint,
  Input,
} from "../../../../common/presentation/components";
import { useSearchField } from "../../hooks/useSearchField";
import classes from "./index.module.scss";
import { handshake } from "../../api/gatewayApi";
export const path = "/case-search";

const Page: React.FC = () => {
  const queryParamState = useQueryParamsState<CaseFilterQueryParams>();
  const reduxState = useSearchDataState(queryParamState.params.urn);
  const searchState = useSearchState(queryParamState, reduxState);

  const { handleChange, handleKeyPress, handleSubmit, urn, isError } =
    useSearchField(searchState);

  useEffect(() => {
    handshake(18846).then((result) => console.log(result));
  }, []);

  return (
    <div className="govuk-grid-row">
      <div className="govuk-grid-column-two-thirds">
        <h1 className="govuk-heading-xl">
          Find a case
          <Hint className={classes.hint}>
            Search and review a CPS case in England
          </Hint>
        </h1>

        <div className="govuk-form-group">
          <Input
            onChange={handleChange}
            onKeyPress={handleKeyPress}
            value={urn}
            autoFocus
            data-testid="input-search-urn"
            errorMessage={
              isError ? { children: "Please enter a valid URN" } : undefined
            }
            label={{
              className: "govuk-label--s",
              children: "Search for a case URN",
            }}
          />
        </div>
        <Button onClick={handleSubmit} data-testid="button-search">
          Search
        </Button>
      </div>
    </div>
  );
};

export default Page;

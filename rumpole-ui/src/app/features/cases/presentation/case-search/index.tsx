import React from "react";
import { useQueryParamsState } from "../../../../common/hooks/useQueryParamsState";
import { CaseFilterQueryParams } from "../../hooks/types/CaseFilterQueryParams";
import {
  Button,
  Hint,
  Input,
} from "../../../../common/presentation/components";
import { useSearchInputLogic } from "../../hooks/useSearchInputLogic";
import classes from "./index.module.scss";

export const path = "/case-search";

const Page: React.FC = () => {
  const { urn: initialUrn, setParams } =
    useQueryParamsState<CaseFilterQueryParams>();

  const { handleChange, handleKeyPress, handleSubmit, isError, urn } =
    useSearchInputLogic({ initialUrn, setParams });

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
              isError
                ? {
                    children: (
                      <span data-testid="input-search-urn-error">
                        Please enter a valid URN
                      </span>
                    ),
                  }
                : undefined
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

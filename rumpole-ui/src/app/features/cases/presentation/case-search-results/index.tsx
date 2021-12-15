import { useState, KeyboardEvent } from "react";
import {
  Button,
  Input,
  Radios,
} from "../../../../common/presentation/components";
import { CaseFilterQueryParams } from "../../hooks/types/CaseFilterQueryParams";
import { useQueryParamsState } from "../../hooks/useQueryParams";
import { useSearchDataState } from "../../hooks/useSearchDataState";
import { useSearchState } from "../../hooks/useSearchState";
import { isUrnValid } from "../../logic/isUrnValid";
import classes from "./index.module.scss";

export const path = "/case-search-results";

const Page: React.FC = () => {
  const searchFilterState = useQueryParamsState<CaseFilterQueryParams>();
  const searchDataState = useSearchDataState(searchFilterState.params.urn);

  const {
    urn: initialUrn,
    filterItems,
    chargedStatus,
    setChargedStatus,
    setUrnParam,
  } = useSearchState(searchFilterState, searchDataState);

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
      <div className="govuk-grid-row">
        <div className="govuk-grid-column-full">
          <h1 className="govuk-heading-xl">Find a case</h1>

          <div className={classes.search}>
            <Input
              onChange={handleChange}
              onKeyPress={handleKeyPress}
              className="govuk-input "
              autoFocus
              label={{
                className: "govuk-label--s",
                children: "Search for a case URN",
              }}
              formGroup={{
                className: "govuk-!-width-one-half",
              }}
              value={urn}
            />
            <Button onClick={handleSubmit} disabled={!isValid}>
              Search
            </Button>
          </div>
        </div>
      </div>

      <div className={`govuk-grid-row ${classes.results}`}>
        <div className="govuk-grid-column-one-third">
          <div className={classes.filter}>
            <div className="inner-block">
              <h3 className="govuk-heading-m">Filter</h3>

              <label className="govuk-label govuk-label--s legend">
                Case status
              </label>

              <Radios
                name="chargedStatus"
                value={chargedStatus}
                items={filterItems}
                onChange={setChargedStatus}
              />
            </div>
          </div>
        </div>
        <div className="govuk-grid-column-two-thirds"></div>
      </div>
    </>
  );
};

export default Page;

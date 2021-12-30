import {
  Button,
  ErrorSummary,
  Hint,
  Input,
  Radios,
} from "../../../../common/presentation/components";
import { CaseFilterQueryParams } from "../../hooks/types/CaseFilterQueryParams";
import { useQueryParamsState } from "../../../../common/hooks/useQueryParamsState";
import { useSearchDataState } from "../../hooks/useSearchDataState";
import { useSearchState } from "../../hooks/useSearchState";
import classes from "./index.module.scss";
import { useSearchField } from "../../hooks/useSearchField";
import { generatePath, Link } from "react-router-dom";
import { path as casePath } from "../case";

export const path = "/case-search-results";

const Page: React.FC = () => {
  const queryParamsState = useQueryParamsState<CaseFilterQueryParams>();
  const reduxState = useSearchDataState(queryParamsState.params.urn);
  const searchState = useSearchState(queryParamsState, reduxState);

  const { handleChange, handleKeyPress, handleSubmit, urn, isError } =
    useSearchField(searchState);

  const {
    chargedStatus,
    chargedStatusFilterItems,
    setChargedStatus,
    loadingStatus,
    totalCount,
    filteredData,
    error,
  } = searchState;

  if (loadingStatus === "loading" || loadingStatus === "idle") {
    return <h1 className="govuk-heading-xl">Please wait...</h1>;
  }

  if (loadingStatus === "failed") {
    return <ErrorSummary descriptionChildren={error} />;
  }

  return (
    <>
      <div className="govuk-grid-row">
        <div className="govuk-grid-column-full">
          <h1 className="govuk-heading-xl">Find a case</h1>

          <div className={classes.search}>
            <Input
              onChange={handleChange}
              onKeyPress={handleKeyPress}
              value={urn}
              autoFocus
              errorMessage={
                isError ? { children: "Please enter a valid URN" } : undefined
              }
              label={{
                className: "govuk-label--s",
                children: "Search for a case URN",
              }}
              formGroup={{
                className: "govuk-!-width-one-half",
              }}
            />
            <Button onClick={handleSubmit}>Search</Button>
          </div>
        </div>
      </div>

      <div className={`govuk-grid-row ${classes.resultsAndFilter}`}>
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
                items={chargedStatusFilterItems}
                onChange={setChargedStatus}
              />
            </div>
          </div>
        </div>

        <div className="govuk-grid-column-two-thirds">
          <div className={classes.results}>
            <p className="govuk-body">
              We've found <b>{totalCount}</b> case
              {totalCount !== 1 ? "s " : " "}
              that match {urn}
            </p>
            <hr className="govuk-section-break govuk-section-break--m govuk-section-break--visible" />
            {filteredData.map((item) => (
              <div key={item.id} className={classes.result}>
                <h2 className="govuk-heading-m ">
                  <Link to={generatePath(casePath, { id: item.id })}>
                    {item.uniqueReferenceNumber}
                  </Link>
                  <Hint className={classes.defendantName}>
                    {item.leadDefendant.surname},{" "}
                    {item.leadDefendant.firstNames}
                  </Hint>
                </h2>
                <div className="govuk-body">
                  <div>
                    <span>Date of offense:</span>
                    <span>{item.offences[0].earlyDate}</span>
                  </div>
                  <div>
                    <span>Charges:</span>
                    <span>{item.offences[0].shortDescription}</span>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </>
  );
};

export default Page;

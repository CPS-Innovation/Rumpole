import {
  BackLink,
  Button,
  ErrorSummary,
  Hint,
  Input,
} from "../../../../common/presentation/components";
import { CaseSearchQueryParams } from "../../types/CaseSearchQueryParams";
import { useQueryParamsState } from "../../../../common/hooks/useQueryParamsState";

import classes from "./index.module.scss";
import { useSearchInputLogic } from "../../hooks/useSearchInputLogic";
import { generatePath, Link } from "react-router-dom";
import { path as casePath } from "../case-details";
import {
  formatDate,
  CommonDateTimeFormats,
} from "../../../../common/utils/dates";
import { BackLinkingPageProps } from "../../../../common/presentation/types/BackLinkingPageProps";
import { PageContentWrapper } from "../../../../common/presentation/components/PageContentWrapper";
import { Wait } from "../../../../common/presentation/components/Wait";
import { useApi } from "../../../../common/hooks/useApi";
import { searchUrn } from "../../api/gatewayApi";
import { Placeholder } from "../../../../common/presentation/components/Placeholder";

export const path = "/case-search-results";

type Props = BackLinkingPageProps & {};

const Page: React.FC<Props> = ({ backLinkProps }) => {
  const {
    urn: initialUrn,
    setParams,
    search,
  } = useQueryParamsState<CaseSearchQueryParams>();

  const { handleChange, handleKeyPress, handleSubmit, isError, urn } =
    useSearchInputLogic({ initialUrn, setParams });

  const state = useApi(searchUrn, initialUrn!);
  console.log(state.status);
  if (state.status === "loading") {
    return <Wait />;
  }

  if (state.status === "failed") {
    return <ErrorSummary descriptionChildren={state.error} />;
  }

  const { data } = state;

  return (
    <>
      <BackLink to={backLinkProps.to}>{backLinkProps.label}</BackLink>
      <PageContentWrapper>
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
        <div className="govuk-grid-row">
          <div className="govuk-grid-column-two-thirds">
            <div className={classes.results}>
              <p className="govuk-body">
                We've found <b data-testid="txt-result-count">{data.length}</b>
                {data.length !== 1
                  ? " cases that match "
                  : " case that matches "}
                <span data-testid="txt-result-urn">{initialUrn}</span>
              </p>
              <hr className="govuk-section-break govuk-section-break--m govuk-section-break--visible" />
              {data.map((item) => (
                <div key={item.id} className={classes.result}>
                  <h2 className="govuk-heading-m ">
                    <Link
                      to={{
                        pathname: generatePath(casePath, { id: item.id }),
                        state: search,
                      }}
                      data-testid={`link-${item.uniqueReferenceNumber}`}
                      className="govuk-link"
                    >
                      {item.uniqueReferenceNumber}
                    </Link>
                    <Hint className={classes.defendantName}>
                      {item.leadDefendant.surname},{" "}
                      {item.leadDefendant.firstNames}
                    </Hint>
                    <Placeholder
                      height={30}
                      width={200}
                      marginTop={-15}
                      marginBottom={0}
                    />
                  </h2>
                  <div className="govuk-body">
                    {item.offences.map((offence, index) => (
                      <div key={index} className={classes["result-offence"]}>
                        <div className={classes["result-offence-line"]}>
                          <span>Status:</span>
                          <span>
                            {offence.isNotYetCharged
                              ? "Not yet charged"
                              : "Charged"}
                          </span>
                        </div>
                        <div className={classes["result-offence-line"]}>
                          <span>Date of offense:</span>
                          <span>
                            {formatDate(
                              offence.earlyDate,
                              CommonDateTimeFormats.ShortDateTextMonth
                            )}
                          </span>
                        </div>
                        <div className={classes["result-offence-line"]}>
                          <span>
                            {offence.isNotYetCharged ? "Proposed" : ""} Charges:
                          </span>
                          <span>{offence.shortDescription}</span>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </PageContentWrapper>
    </>
  );
};

export default Page;

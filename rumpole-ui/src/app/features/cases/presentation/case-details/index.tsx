import { useParams } from "react-router-dom";
import {
  BackLink,
  ErrorSummary,
} from "../../../../common/presentation/components";
import { PageContentWrapper } from "../../../../common/presentation/components/PageContentWrapper";
import { Wait } from "../../../../common/presentation/components/Wait";
import { BackLinkingPageProps } from "../../../../common/presentation/types/BackLinkingPageProps";
import { useCaseDetailsReduxState } from "../../hooks/useCaseDetailsReduxState";
import classes from "./index.module.scss";
export const path = "/case-details/:id";

type Props = BackLinkingPageProps & {};

export const Page: React.FC<Props> = ({ backLinkProps }) => {
  const { id } = useParams<{ id?: string }>();

  const { loadingStatus, error, item } = useCaseDetailsReduxState(id);

  if (loadingStatus === "loading" || loadingStatus === "idle") {
    return <Wait />;
  }

  if (loadingStatus === "failed") {
    return <ErrorSummary descriptionChildren={error} />;
  }

  return (
    <>
      <BackLink to={backLinkProps.to}>{backLinkProps.label}</BackLink>

      <PageContentWrapper>
        <div className="govuk-grid-row">
          <div className="govuk-grid-column-one-quarter">
            <h1
              className={`govuk-heading-m ${classes.defendantName}`}
              data-testid="txt-defendant-name"
            >
              {item?.leadDefendant.surname}, {item?.leadDefendant.firstNames}
            </h1>
            <span className="govuk-caption-m" data-testid="txt-case-urn">
              {item?.uniqueReferenceNumber}
            </span>
          </div>
          <div className="govuk-grid-column-three-quarters"></div>
        </div>
      </PageContentWrapper>
    </>
  );
};

export default Page;

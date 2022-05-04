import { useMemo } from "react";
import { useParams } from "react-router-dom";
import { useApi } from "../../../../common/hooks/useApi";
import { BackLink } from "../../../../common/presentation/components";
import { PageContentWrapper } from "../../../../common/presentation/components/PageContentWrapper";
import { Placeholder } from "../../../../common/presentation/components/Placeholder";
import { Wait } from "../../../../common/presentation/components/Wait";
import { BackLinkingPageProps } from "../../../../common/presentation/types/BackLinkingPageProps";
import { getCaseDetails, getCaseDocuments } from "../../api/gateway-api";
import { Accordion } from "./accordion/Accordion";
import { documentsMapper } from "./accordion/documents-mapper";

import classes from "./index.module.scss";
import { KeyDetails } from "./KeyDetails";
export const path = "/case-details/:id";

type Props = BackLinkingPageProps & {};

export const Page: React.FC<Props> = ({ backLinkProps }) => {
  const { id } = useParams<{ id: string }>();

  const state = useApi(getCaseDetails, id);
  const documentsState = useApi(getCaseDocuments, id);

  const mappedDocumentSections = useMemo(
    () => documentsMapper(documentsState),
    [documentsState]
  );

  if (state.status === "loading") {
    // if we are waiting on the main case details call, show holding message
    //  (we are prepared to show page whilst waiting for docs to load though)
    return <Wait />;
  }

  if (state.status === "failed") {
    throw state.error;
  }

  if (documentsState.status === "failed") {
    throw documentsState.error;
  }

  const { data } = state;

  return (
    <>
      <Placeholder height={40} />
      <BackLink to={backLinkProps.to}>{backLinkProps.label}</BackLink>

      <PageContentWrapper>
        <div className={`govuk-grid-row ${classes.mainContent}`}>
          <div
            className={`govuk-grid-column-one-quarter ${classes.leftColumn}`}
          >
            <KeyDetails caseDetails={data} />

            <Placeholder
              height={200}
              marginTop={20}
              backgroundColor={"#1d70b8"}
            />

            <Placeholder height={40} marginTop={50} marginBottom={20} />

            {!mappedDocumentSections ? (
              <div className="govuk-body">
                Documents loading, Please wait...
              </div>
            ) : (
              <Accordion sections={mappedDocumentSections} />
            )}
          </div>
          <div className="govuk-grid-column-three-quarters"></div>
        </div>
      </PageContentWrapper>
    </>
  );
};

export default Page;

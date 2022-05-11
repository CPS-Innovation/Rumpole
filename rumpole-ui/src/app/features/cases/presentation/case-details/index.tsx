import { useCallback, useMemo, useState } from "react";
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
import { KeyDetails } from "./KeyDetails";
import classes from "./index.module.scss";
import { PdfTabs } from "./pdf-tabs/PdfTabs";
import { CaseDocument } from "../../domain/CaseDocument";

export const path = "/case-details/:id";

type Props = BackLinkingPageProps & {};

export const Page: React.FC<Props> = ({ backLinkProps }) => {
  const { id } = useParams<{ id: string }>();

  const caseState = useApi(getCaseDetails, id);
  const documentsState = useApi(getCaseDocuments, id);

  const mappedDocumentSections = useMemo(
    () => documentsMapper(documentsState),
    [documentsState]
  );

  const [openDocumentsState, setOpenDocumentsState] = useState<CaseDocument[]>(
    []
  );

  const handleOpenDocument = useCallback(
    (caseDocument: CaseDocument) => {
      if (
        openDocumentsState.some(
          (item) => item.documentId === caseDocument.documentId
        )
      ) {
        return;
      }
      console.log(openDocumentsState);
      setOpenDocumentsState([...openDocumentsState, caseDocument]);
    },

    [openDocumentsState]
  );

  if (caseState.status === "loading") {
    // if we are waiting on the main case details call, show holding message
    //  (we are prepared to show page whilst waiting for docs to load though)
    return <Wait />;
  }

  if (caseState.status === "failed") {
    throw caseState.error;
  }

  if (documentsState.status === "failed") {
    throw documentsState.error;
  }

  const { data } = caseState;

  return (
    <>
      <Placeholder height={40} />

      <BackLink to={backLinkProps.to}>{backLinkProps.label}</BackLink>

      <PageContentWrapper>
        <div className={`govuk-grid-row ${classes.mainContent}`}>
          <div
            className={`govuk-grid-column-one-quarter perma-scrollbar ${classes.leftColumn}`}
          >
            <div>
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
                <Accordion
                  sections={mappedDocumentSections}
                  handleOpenDocument={handleOpenDocument}
                />
              )}
            </div>
          </div>
          <div
            className={`govuk-grid-column-three-quarters ${classes.rightColumn}`}
          >
            <PdfTabs items={openDocumentsState} />
          </div>
        </div>
      </PageContentWrapper>
    </>
  );
};

export default Page;

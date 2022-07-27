import { useParams } from "react-router-dom";
import { BackLink } from "../../../../common/presentation/components";
import { PageContentWrapper } from "../../../../common/presentation/components";
import { Placeholder } from "../../../../common/presentation/components";
import { WaitPage } from "../../../../common/presentation/components";
import { Wait as AccordionWait } from "./accordion/Wait";
import { BackLinkingPageProps } from "../../../../common/presentation/types/BackLinkingPageProps";
import { Accordion } from "./accordion/Accordion";
import { KeyDetails } from "./KeyDetails";
import classes from "./index.module.scss";
import { PdfTabs } from "./pdf-tabs/PdfTabs";
import { useCaseDetailsState } from "../../hooks/use-case-details-state/useCaseDetailsState";
import { PdfTabsEmpty } from "./pdf-tabs/PdfTabsEmpty";
import { SearchBox } from "./search-box/SearchBox";
import { ResultsModal } from "./results/ResultsModal";

export const path = "/case-details/:id";

type Props = BackLinkingPageProps & {};

export const Page: React.FC<Props> = ({ backLinkProps }) => {
  const { id } = useParams<{ id: string }>();

  const {
    caseState,
    accordionState,
    tabsState,
    searchState,
    searchTerm,
    pipelineState,
    handleOpenPdf,
    handleClosePdf,
    handleSearchTermChange,
    handleLaunchSearchResults,
    handleCloseSearchResults,
    handleChangeResultsOrder,
    handleUpdateFilter,
  } = useCaseDetailsState(id);

  if (caseState.status === "loading") {
    // if we are waiting on the main case details call, show holding message
    //  (we are prepared to show page whilst waiting for docs to load though)
    return <WaitPage />;
  }

  return (
    <>
      {searchState.isResultsVisible && (
        <ResultsModal
          {...{
            caseState,
            searchTerm,
            searchState,
            pipelineState,
            handleSearchTermChange,
            handleLaunchSearchResults,
            handleCloseSearchResults,
            handleChangeResultsOrder,
            handleUpdateFilter,
            handleOpenPdf,
          }}
        />
      )}

      <Placeholder height={40} />

      <BackLink to={backLinkProps.to}>{backLinkProps.label}</BackLink>

      <PageContentWrapper>
        <div className={`govuk-grid-row ${classes.mainContent}`}>
          <div
            className={`govuk-grid-column-one-quarter perma-scrollbar ${classes.leftColumn}`}
          >
            <div>
              <KeyDetails caseDetails={caseState.data} />

              <Placeholder
                height={200}
                marginTop={20}
                backgroundColor={"#1d70b8"}
              />

              <SearchBox
                data-testid="search-case"
                labelText="Search"
                value={searchTerm}
                handleChange={handleSearchTermChange}
                handleSubmit={handleLaunchSearchResults}
              />

              {accordionState.status === "loading" ? (
                <AccordionWait />
              ) : (
                <Accordion
                  accordionState={accordionState.data}
                  handleOpenPdf={(caseDoc) =>
                    handleOpenPdf({ ...caseDoc, mode: "read" })
                  }
                />
              )}
            </div>
          </div>
          <div
            className={`govuk-grid-column-three-quarters ${classes.rightColumn}`}
          >
            {!tabsState.items.length ? (
              <PdfTabsEmpty />
            ) : (
              <PdfTabs
                tabsState={tabsState}
                handleClosePdf={handleClosePdf}
                handleLaunchSearchResults={handleLaunchSearchResults}
              />
            )}
          </div>
        </div>
      </PageContentWrapper>
    </>
  );
};

export default Page;

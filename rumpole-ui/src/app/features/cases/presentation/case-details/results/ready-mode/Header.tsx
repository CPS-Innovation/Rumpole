import {
  SectionBreak,
  Select,
} from "../../../../../../common/presentation/components";
import { CombinedState } from "../../../../domain/CombinedState";
import { MappedTextSearchResult } from "../../../../domain/MappedTextSearchResult";
import { CaseDetailsState } from "../../../../hooks/use-case-details-state/useCaseDetailsState";
import { MissingDocs } from "./MissingDocs";
import classes from "./Header.module.scss";

type ResultsOrder = CaseDetailsState["searchState"]["resultsOrder"];

type Props = {
  submittedSearchTerm: string;
  searchResult: MappedTextSearchResult;
  missingDocs: CombinedState["searchState"]["missingDocs"];
  resultsOrder: ResultsOrder;
  handleChangeResultsOrder: CaseDetailsState["handleChangeResultsOrder"];
};

export const Header: React.FC<Props> = ({
  searchResult: { filteredDocumentCount, filteredOccurrencesCount },
  missingDocs,
  submittedSearchTerm,
  handleChangeResultsOrder,
  resultsOrder,
}) => {
  const isMissingDocsMode = !!missingDocs.length;

  const items: {
    children: string;
    value: ResultsOrder;
  }[] = [
    {
      children: "Date added",
      value: "byDateDesc" as const,
    },
    {
      children: "Results per document",
      value: "byOccurancesPerDocumentDesc" as const,
    },
  ];

  return (
    <>
      <div className={classes.container}>
        <div className={classes.textSection}>
          <div data-testid="div-results-header">
            <div className={classes.singleWordSearchWarning}>
              Search can only use the first word of your search and may no have
              found all instances of '{submittedSearchTerm}' in this case.
            </div>

            {!!filteredDocumentCount ? (
              <>
                {" "}
                <b>{filteredOccurrencesCount}</b> results in{" "}
                <b>{filteredDocumentCount}</b>{" "}
                {`document${filteredDocumentCount ? "s" : ""} `}
              </>
            ) : (
              <>No documents found matching '{submittedSearchTerm}'</>
            )}
          </div>
          {isMissingDocsMode && (
            <div>
              Search may not have found all instances of '{submittedSearchTerm}'
              in this case
            </div>
          )}
        </div>
        {!!filteredDocumentCount && (
          <Select
            data-testid="select-result-order"
            value={resultsOrder}
            items={items}
            formGroup={{
              className: classes.select,
            }}
            onChange={(ev) =>
              handleChangeResultsOrder(ev.target.value as ResultsOrder)
            }
          />
        )}
      </div>

      <SectionBreak noTopMargin />

      {isMissingDocsMode && (
        <>
          <MissingDocs missingDocs={missingDocs} />
          <SectionBreak />
        </>
      )}
    </>
  );
};

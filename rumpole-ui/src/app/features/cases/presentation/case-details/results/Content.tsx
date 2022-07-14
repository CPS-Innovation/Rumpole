import { CaseDetailsState } from "../../../hooks/use-case-details-state/useCaseDetailsState";
import { SearchBox } from "../search-box/SearchBox";
import { ContentModeSwitch } from "./ContentModeSwitch";
import classes from "./Content.module.scss";
import { SucceededApiResult } from "../../../../../common/types/SucceededApiResult";
import { CaseDetails } from "../../../domain/CaseDetails";
type Props = {
  caseState: SucceededApiResult<CaseDetails>;
  searchTerm: CaseDetailsState["searchTerm"];
  pipelineState: CaseDetailsState["pipelineState"];
  searchState: CaseDetailsState["searchState"];
  handleSearchTermChange: CaseDetailsState["handleSearchTermChange"];
  handleLaunchSearchResults: CaseDetailsState["handleLaunchSearchResults"];
  handleChangeResultsOrder: CaseDetailsState["handleChangeResultsOrder"];
  handleUpdateFilter: CaseDetailsState["handleUpdateFilter"];
  handleOpenPdf: CaseDetailsState["handleOpenPdf"];
};

export const Content: React.FC<Props> = ({
  caseState: {
    data: {
      leadDefendant: { surname },
      uniqueReferenceNumber,
    },
  },
  searchTerm: value,
  pipelineState,
  searchState,
  handleSearchTermChange: handleChange,
  handleLaunchSearchResults: handleSubmit,
  handleChangeResultsOrder,
  handleUpdateFilter,
  handleOpenPdf,
}) => {
  const labelText = `Search ${surname}, ${uniqueReferenceNumber}`;

  return (
    <div
      className={`govuk-width-container ${classes.contentWidth}`}
      data-testid="div-search-results"
    >
      <div className="govuk-grid-row">
        <div className="govuk-!-width-one-half">
          <SearchBox
            {...{ labelText, value, handleChange, handleSubmit }}
            data-testid="results-search-case"
          />
        </div>
      </div>
      <div className="govuk-grid-row">
        <ContentModeSwitch
          {...{
            pipelineState,
            searchState,
            handleChangeResultsOrder,
            handleUpdateFilter,
            handleOpenPdf,
          }}
        />
      </div>
    </div>
  );
};

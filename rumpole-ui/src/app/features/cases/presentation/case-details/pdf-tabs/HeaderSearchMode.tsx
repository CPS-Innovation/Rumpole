import { LinkButton } from "../../../../../common/presentation/components/LinkButton";
import { CaseDocumentViewModel } from "../../../domain/CaseDocumentViewModel";
import classes from "./HeaderSearchMode.module.scss";

type Props = {
  caseDocumentViewModel: Extract<CaseDocumentViewModel, { mode: "search" }>;
  handleLaunchSearchResults: () => void;
  focussedHighlightIndex: number;
  handleSetFocussedHighlightIndex: (nextIndex: number) => void;
};

export const HeaderSearchMode: React.FC<Props> = ({
  caseDocumentViewModel: { searchTerm, occurrencesInDocumentCount, fileName },
  focussedHighlightIndex,
  handleSetFocussedHighlightIndex,
  handleLaunchSearchResults,
}) => {
  const oneBasedFocussedHighlightIndex = focussedHighlightIndex + 1;

  return (
    <div className={classes.content}>
      <div className={classes.heavyText}>
        <LinkButton onClick={handleLaunchSearchResults}>
          Back to search results
        </LinkButton>
      </div>
      <div className={classes.heavyText}>
        {occurrencesInDocumentCount}{" "}
        {occurrencesInDocumentCount === 1 ? "match" : "matches"} for "
        {searchTerm}" in {fileName}
      </div>

      <div>
        {focussedHighlightIndex > 0 && (
          <LinkButton
            onClick={() =>
              handleSetFocussedHighlightIndex(focussedHighlightIndex - 1)
            }
          >
            Previous
          </LinkButton>
        )}{" "}
        {oneBasedFocussedHighlightIndex}/{occurrencesInDocumentCount}{" "}
        {focussedHighlightIndex < occurrencesInDocumentCount - 1 && (
          <LinkButton
            onClick={() =>
              handleSetFocussedHighlightIndex(focussedHighlightIndex + 1)
            }
          >
            Next
          </LinkButton>
        )}
      </div>
    </div>
  );
};

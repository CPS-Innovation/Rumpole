import { LinkButton } from "../../../../../common/presentation/components/LinkButton";
import classes from "./HeaderSearchModeNavigation.module.scss";

type Props = {
  occurrencesInDocumentCount: number;
  focussedHighlightIndex: number;
  handleSetFocussedHighlightIndex: (nextIndex: number) => void;
};

export const HeaderSearchModeNavigation: React.FC<Props> = ({
  occurrencesInDocumentCount,
  focussedHighlightIndex,
  handleSetFocussedHighlightIndex,
}) => {
  const oneBasedFocussedHighlightIndex = focussedHighlightIndex + 1;

  return (
    <div className={classes.container}>
      <span className={classes.previous}>
        {focussedHighlightIndex > 0 ? (
          <LinkButton
            onClick={() =>
              handleSetFocussedHighlightIndex(focussedHighlightIndex - 1)
            }
          >
            Previous
          </LinkButton>
        ) : (
          <span>Previous</span>
        )}
      </span>
      <span className={classes.numbers}>
        {oneBasedFocussedHighlightIndex}/{occurrencesInDocumentCount}
      </span>
      <span className={classes.next}>
        {focussedHighlightIndex < occurrencesInDocumentCount - 1 ? (
          <LinkButton
            onClick={() =>
              handleSetFocussedHighlightIndex(focussedHighlightIndex + 1)
            }
          >
            Next
          </LinkButton>
        ) : (
          <span>Next</span>
        )}
      </span>
    </div>
  );
};

import { useCallback } from "react";
import { useMemo, useState } from "react";
import { CaseDocumentViewModel } from "../../../domain/CaseDocumentViewModel";
import { NewPdfHighlight } from "../../../domain/NewPdfHighlight";
import { CaseDetailsState } from "../../../hooks/use-case-details-state/useCaseDetailsState";
import { PdfViewer } from "../pdf-viewer/PdfViewer";
import { Wait } from "../pdf-viewer/Wait";
import { HeaderReadMode } from "./HeaderReadMode";
import { HeaderSearchMode } from "./HeaderSearchMode";

type PdfTabProps = {
  caseDocumentViewModel: CaseDocumentViewModel;
  authToken: string | undefined;
  handleLaunchSearchResults: () => void;
  handleAddRedaction: CaseDetailsState["handleAddRedaction"];
  handleRemoveRedaction: CaseDetailsState["handleRemoveRedaction"];
};

export const PdfTab: React.FC<PdfTabProps> = ({
  caseDocumentViewModel,
  authToken,
  handleLaunchSearchResults,
  handleAddRedaction,
  handleRemoveRedaction,
}) => {
  const [focussedHighlightIndex, setFocussedHighlightIndex] =
    useState<number>(1);

  const { url, mode, redactionHighlights, documentId } = caseDocumentViewModel;

  const searchHighlights =
    mode === "search" ? caseDocumentViewModel.searchHighlights : undefined;

  const highlights = useMemo(
    () => [...(searchHighlights || []), ...redactionHighlights],
    [searchHighlights, redactionHighlights]
  );

  const localHandleAddRedaction = useCallback(
    (redaction: NewPdfHighlight) => handleAddRedaction(documentId, redaction),
    [documentId, handleAddRedaction]
  );

  const localHandleRemoveRedaction = useCallback(
    (redactionId: string) => handleRemoveRedaction(documentId, redactionId),
    [documentId, handleRemoveRedaction]
  );

  return (
    <>
      {mode === "search" ? (
        <HeaderSearchMode
          caseDocumentViewModel={caseDocumentViewModel}
          handleLaunchSearchResults={handleLaunchSearchResults}
          focussedHighlightIndex={focussedHighlightIndex}
          handleSetFocussedHighlightIndex={setFocussedHighlightIndex}
        />
      ) : (
        <HeaderReadMode />
      )}

      {url && authToken ? (
        <PdfViewer
          url={url}
          authToken={authToken}
          highlights={highlights}
          focussedHighlightIndex={focussedHighlightIndex}
          handleAddRedaction={localHandleAddRedaction}
          handleRemoveRedaction={localHandleRemoveRedaction}
        />
      ) : (
        <Wait />
      )}
    </>
  );
};

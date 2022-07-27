import { useState } from "react";
import { CaseDocumentViewModel } from "../../../domain/CaseDocumentViewModel";
import PdfViewer from "../pdf-viewer/PdfViewer";
import { Wait } from "../pdf-viewer/Wait";
import { HeaderReadMode } from "./HeaderReadMode";
import { HeaderSearchMode } from "./HeaderSearchMode";
import { mapHighlights } from "./map-highlights";

type PdfTabProps = {
  caseDocumentViewModel: CaseDocumentViewModel;
  authToken: string | undefined;
  handleLaunchSearchResults: () => void;
};

export const PdfTab: React.FC<PdfTabProps> = ({
  caseDocumentViewModel,
  authToken,
  handleLaunchSearchResults,
}) => {
  const [focussedHighlightIndex, setFocussedHighlightIndex] =
    useState<number>(1);

  const { url, mode } = caseDocumentViewModel;

  const highlights =
    mode === "search" ? mapHighlights(caseDocumentViewModel) : [];

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
        />
      ) : (
        <Wait />
      )}
    </>
  );
};

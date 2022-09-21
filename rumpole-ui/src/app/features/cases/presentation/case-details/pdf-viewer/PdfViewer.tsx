import React, { useCallback, useEffect, useMemo, useRef } from "react";

import {
  PdfLoader,
  PdfHighlighter,
  Popup,
  ScaledPosition,
  IHighlight,
} from "../../../../../../react-pdf-highlighter";

import classes from "./PdfViewer.module.scss";
import { Wait } from "./Wait";
import { RedactButton } from "./RedactButton";

import { PdfLinearHighlight } from "./PdfLinearHighlight";
import { PdfAreaHighlight } from "./PdfAreaHighlight";
import { RemoveButton } from "./RemoveButton";
import { IPdfHighlight } from "../../../domain/IPdfHighlight";
import { NewPdfHighlight } from "../../../domain/NewPdfHighlight";
import { Footer } from "./Footer";

const SCROLL_TO_OFFSET = 120;

type Props = {
  url: string;
  authToken: string;
  searchHighlights: undefined | IPdfHighlight[];
  redactionHighlights: IPdfHighlight[];
  focussedHighlightIndex: number;
  handleAddRedaction: (newRedaction: NewPdfHighlight) => void;
  handleRemoveRedaction: (id: string) => void;
  handleRemoveAllRedactions: () => void;
  handleSavedRedactions: () => void;
};

const ensureAllPdfInView = () =>
  window.scrollY < SCROLL_TO_OFFSET &&
  window.scrollTo({ top: SCROLL_TO_OFFSET });

export const PdfViewer: React.FC<Props> = ({
  url,
  authToken,
  searchHighlights,
  redactionHighlights,
  handleAddRedaction,
  handleRemoveRedaction,
  handleRemoveAllRedactions,
  handleSavedRedactions,
  focussedHighlightIndex,
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const scrollToFnRef = useRef<(highlight: IHighlight) => void>();

  const highlights = useMemo(
    () => [...(searchHighlights || []), ...redactionHighlights],
    [searchHighlights, redactionHighlights]
  );

  useEffect(() => {
    scrollToFnRef.current &&
      scrollToFnRef.current(highlights[focussedHighlightIndex]);
  }, [highlights, focussedHighlightIndex]);

  const addRedaction = useCallback(
    (position: ScaledPosition, isAreaHighlight: boolean) => {
      const newRedaction: NewPdfHighlight = {
        type: "redaction",
        position,
        highlightType: isAreaHighlight ? "area" : "linear",
      };

      handleAddRedaction(newRedaction);
    },
    [handleAddRedaction]
  );

  return (
    <>
      <div
        className={classes.pdfViewer}
        ref={containerRef}
        data-testid="div-pdfviewer"
      >
        <PdfLoader url={url} authToken={authToken} beforeLoad={<Wait />}>
          {(pdfDocument) => (
            <PdfHighlighter
              onWheelDownwards={ensureAllPdfInView}
              pdfDocument={pdfDocument}
              enableAreaSelection={(event) =>
                (event.target as HTMLElement).className === "textLayer"
              }
              onScrollChange={() => {}}
              pdfScaleValue="page-width"
              scrollRef={(scrollTo) => {
                scrollToFnRef.current = scrollTo;
                // imperatively trigger as soon as we have reference to the scrollTo function
                scrollTo(highlights[0]);
              }}
              onSelectionFinished={(position, content, hideTipAndSelection) => (
                <RedactButton
                  onConfirm={() => {
                    addRedaction(position, !!content.image);
                    hideTipAndSelection();
                  }}
                />
              )}
              highlightTransform={(
                highlight,
                index,
                setTip,
                hideTip,
                _,
                __,
                isScrolledTo
              ) => {
                const component =
                  highlight.highlightType === "linear" ? (
                    <PdfLinearHighlight
                      type={highlight.type}
                      isScrolledTo={isScrolledTo}
                      position={highlight.position}
                    />
                  ) : (
                    <PdfAreaHighlight
                      isScrolledTo={isScrolledTo}
                      position={highlight.position}
                    />
                  );

                return highlight.type === "search" ? (
                  { ...component, key: index }
                ) : (
                  <Popup
                    popupContent={
                      <RemoveButton
                        onClick={() => {
                          handleRemoveRedaction(highlight.id);
                          hideTip();
                        }}
                      />
                    }
                    onMouseOver={(popupContent) =>
                      setTip(highlight, (highlight) => popupContent)
                    }
                    onMouseOut={hideTip}
                    key={index}
                    children={component}
                  />
                );
              }}
              highlights={highlights}
            />
          )}
        </PdfLoader>
        {!!redactionHighlights.length && (
          <Footer
            redactionHighlights={redactionHighlights}
            handleRemoveAllRedactions={handleRemoveAllRedactions}
            handleSavedRedactions={handleSavedRedactions}
          />
        )}
      </div>
    </>
  );
};

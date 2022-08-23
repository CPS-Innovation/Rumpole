import React from "react";
import { Component } from "react";

import {
  PdfLoader,
  PdfHighlighter,
  Popup,
  ScaledPosition,
} from "../../../../../../react-pdf-highlighter";

import classes from "./PdfViewer.module.scss";
import { Wait } from "./Wait";
import { RedactButton } from "./RedactButton";

import { PdfLinearHighlight } from "./PdfLinearHighlight";
import { PdfAreaHighlight } from "./PdfAreaHighlight";
import { RemoveButton } from "./RemoveButton";
import { IPdfHighlight } from "../../../domain/IPdfHighlight";
import { NewPdfHighlight } from "../../../domain/NewPdfHighlight";

const SCROLL_TO_OFFSET = 120;

interface Props {
  url: string;
  authToken: string;
  highlights: IPdfHighlight[];
  focussedHighlightIndex: number;
  handleAddRedaction: (newRedaction: NewPdfHighlight) => void;
  handleRemoveRedaction: (id: string) => void;
}

const resetHash = () => {
  document.location.hash = "";
};

export class PdfViewer extends Component<Props /*, State*/> {
  private containerRef: React.RefObject<HTMLDivElement>;

  constructor(props: Props) {
    super(props);
    this.containerRef = React.createRef();
  }

  addRedaction(position: ScaledPosition, isAreaHighlight: boolean) {
    const newRedaction: NewPdfHighlight = {
      type: "redaction",
      position,
      highlightType: isAreaHighlight ? "area" : "linear",
    };

    this.props.handleAddRedaction(newRedaction);
  }

  removeRedaction = (id: string) => {
    this.props.handleRemoveRedaction(id);
  };

  render() {
    const { highlights } = this.props;
    const { url } = this.props;

    return (
      <>
        <div
          className={classes.pdfViewer}
          ref={this.containerRef}
          data-testid="div-pdfviewer"
        >
          <PdfLoader
            url={url}
            authToken={this.props.authToken}
            beforeLoad={<Wait />}
          >
            {(pdfDocument) => (
              <PdfHighlighter
                onWheelDownwards={() =>
                  window.scrollY < SCROLL_TO_OFFSET &&
                  window.scrollTo({ top: SCROLL_TO_OFFSET })
                }
                pdfDocument={pdfDocument}
                enableAreaSelection={(event) =>
                  (event.target as HTMLElement).className === "textLayer"
                }
                onScrollChange={resetHash}
                pdfScaleValue="page-width"
                scrollRef={(scrollTo) => {
                  //this.scrollViewerTo = scrollTo;
                  //this.scrollToHighlightFromHash();
                }}
                onSelectionFinished={(
                  position,
                  content,
                  hideTipAndSelection
                ) => (
                  <RedactButton
                    onConfirm={() => {
                      this.addRedaction(position, !!content.image);
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
                        type={highlight.type}
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
                            this.removeRedaction(highlight.id);
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
        </div>
      </>
    );
  }
}

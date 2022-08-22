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
import { IPdfHighlight } from "./types/IPdfHighlight";
import { PdfLinearHighlight } from "./PdfLinearHighlight";
import { PdfAreaHighlight } from "./PdfAreaHighlight";
import { RemoveButton } from "./RemoveButton";

const SCROLL_TO_OFFSET = 120;

interface State {
  highlights: IPdfHighlight[];
  redactions: IPdfHighlight[];
}

interface Props {
  url: string;
  authToken: string;
  highlights: IPdfHighlight[];
  focussedHighlightIndex: number;
}

const getNextId = () => String(Math.random()).slice(2);

// const parseIdFromHash = () =>
//   document.location.hash.slice("#highlight-".length);

const resetHash = () => {
  document.location.hash = "";
};

class App extends Component<Props, State> {
  private containerRef: React.RefObject<HTMLDivElement>;

  static getDerivedStateFromProps(props: Props, state: State) {
    return { ...state, highlights: props.highlights };
  }

  constructor(props: Props) {
    super(props);
    this.containerRef = React.createRef();
  }

  state: State = {
    highlights: this.props.highlights,
    redactions: [],
  };

  // scrollToHighlightFromHash = () => {
  //   const highlight = this.getHighlightById(
  //     String(this.props.focussedHighlightIndex)
  //   );

  //   if (highlight) {
  //     this.scrollViewerTo(highlight);
  //   }
  // };

  // componentDidMount() {
  //   window.addEventListener(
  //     "hashchange",
  //     this.scrollToHighlightFromHash,
  //     false
  //   );
  // }

  // getHighlightById(id: string) {
  //   const { highlights } = this.state;

  //   return highlights.find((highlight) => highlight.id === id);
  // }

  resetHighlights = () => {
    this.setState({
      highlights: [],
    });
  };

  //scrollViewerTo = (highlight: any) => {};

  addRedaction(position: ScaledPosition, isAreaHighlight: boolean) {
    const { redactions } = this.state;
    this.setState({
      redactions: [
        {
          id: getNextId(),
          type: "redaction",
          position,
          highlightType: isAreaHighlight ? "area" : "linear",
        },
        ...redactions,
      ],
    });
  }

  removeRedaction = (id: string) => {
    this.setState({
      redactions: this.state.redactions.filter((item) => item.id !== id),
    });
  };

  render() {
    const { highlights, redactions } = this.state;
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
                highlights={[...highlights, ...redactions]}
              />
            )}
          </PdfLoader>
        </div>
      </>
    );
  }
}

export default App;

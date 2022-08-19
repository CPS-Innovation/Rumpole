import React from "react";
import { Component } from "react";

import {
  PdfLoader,
  PdfHighlighter,
  Highlight,
  Popup,
  AreaHighlight,
  IHighlight,
} from "../../../../../../react-pdf-highlighter";

import { IRedaction, NewRedaction } from "./types";

import "./style/pdf-viewer.css"; // todo: these are the styles that come along with pdf-highlighter, get rid of these/transplant
import classes from "../index.module.scss";
import { Wait } from "./Wait";
import { RedactButton } from "./RedactButton";

const SCROLL_TO_OFFSET = 120;

interface State {
  highlights: IHighlight[];
  redactions: IRedaction[];
}

interface Props {
  url: string;
  authToken: string;
  highlights: IHighlight[];
  focussedHighlightIndex: number;
}

const getNextId = () => String(Math.random()).slice(2);

// const parseIdFromHash = () =>
//   document.location.hash.slice("#highlight-".length);

const resetHash = () => {
  document.location.hash = "";
};

const HighlightPopup = ({
  comment,
  onClick,
}: {
  comment: { text: string; emoji: string };
  onClick: () => void;
}) => (
  <div className="Tip">
    <div className="Tip__compact Tip__compact-unredact" onClick={onClick}>
      Remove redaction
    </div>
  </div>
);

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

  resetHighlights = () => {
    this.setState({
      highlights: [],
    });
  };

  scrollViewerTo = (highlight: any) => {};

  scrollToHighlightFromHash = () => {
    const highlight = this.getHighlightById(
      String(this.props.focussedHighlightIndex)
    );

    if (highlight) {
      this.scrollViewerTo(highlight);
    }
  };

  componentDidMount() {
    window.addEventListener(
      "hashchange",
      this.scrollToHighlightFromHash,
      false
    );
  }

  getHighlightById(id: string) {
    const { highlights } = this.state;

    return highlights.find((highlight) => highlight.id === id);
  }

  addRedaction(redaction: NewRedaction) {
    const { redactions } = this.state;
    this.setState({
      redactions: [{ ...redaction, id: getNextId() }, ...redactions],
    });
  }

  removeRedaction = (id: string) => {
    this.setState({
      redactions: this.state.redactions.filter((item) => item.id !== id),
    });
  };

  render() {
    const { highlights } = this.state;
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
                  this.scrollViewerTo = scrollTo;

                  this.scrollToHighlightFromHash();
                }}
                onSelectionFinished={(position, _, hideTipAndSelection) => (
                  <RedactButton
                    onConfirm={() => {
                      this.addRedaction({
                        position,
                      });
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
                  const isTextHighlight = !Boolean(
                    highlight.content && highlight.content.image
                  );

                  const component = isTextHighlight ? (
                    <Highlight
                      isScrolledTo={isScrolledTo}
                      position={highlight.position}
                      comment={highlight.comment}
                    />
                  ) : (
                    <AreaHighlight
                      isScrolledTo={isScrolledTo}
                      highlight={highlight}
                      onChange={(boundingRect) => {
                        //remove this eventually
                      }}
                    />
                  );

                  return (
                    <Popup
                      popupContent={
                        <HighlightPopup
                          {...highlight}
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

export default App;

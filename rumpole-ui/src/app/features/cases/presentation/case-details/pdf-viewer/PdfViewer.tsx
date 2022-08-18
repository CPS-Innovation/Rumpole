import React from "react";
import { Component } from "react";

import {
  PdfLoader,
  PdfHighlighter,
  Highlight,
  Popup,
  AreaHighlight,
  IHighlight,
  NewHighlight,
  //Tip,
} from "../../../../../../react-pdf-highlighter";

import "./style/pdf-viewer.css"; // todo: these are the styles that come along with pdf-highlighter, get rid of these/transplant
import classes from "../index.module.scss";
import { Wait } from "./Wait";

const SCROLL_TO_OFFSET = 120;

interface State {
  url: string;
  highlights: Array<IHighlight>;
}

interface Props {
  url: string;
  authToken: string;
  highlights: Array<IHighlight>;
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

  state = {
    url: this.props.url,
    highlights: this.props.highlights,
    isRedactionComplete: false,
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

  addHighlight(highlight: NewHighlight) {
    const { highlights } = this.state;

    console.log("Saving highlight", highlight);

    this.setState({
      highlights: [{ ...highlight, id: getNextId() }, ...highlights],
    });
  }

  updateHighlight(highlightId: string, position: Object, content: Object) {
    console.log("Updating highlight", highlightId, position, content);

    this.setState({
      highlights: this.state.highlights.map((h) => {
        const {
          id,
          position: originalPosition,
          content: originalContent,
          ...rest
        } = h;
        return id === highlightId
          ? {
              id,
              position: { ...originalPosition, ...position },
              content: { ...originalContent, ...content },
              ...rest,
            }
          : h;
      }),
    });
  }

  removeHighlight = (id: string) => {
    this.setState({
      highlights: this.state.highlights.filter((item) => item.id !== id),
    });
  };

  render() {
    const { url, highlights } = this.state;

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
                onSelectionFinished={(
                  position,
                  content,
                  hideTipAndSelection,
                  transformSelection
                ) => (
                  <></>
                  // <Tip
                  //   onOpen={transformSelection}
                  //   onConfirm={(comment) => {
                  //     this.addHighlight({
                  //       content,
                  //       position,
                  //       comment,
                  //     });
                  //     hideTipAndSelection();
                  //   }}
                  // />
                )}
                highlightTransform={(
                  highlight,
                  index,
                  setTip,
                  hideTip,
                  viewportToScaled,
                  screenshot,
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
                        this.updateHighlight(
                          highlight.id,
                          {
                            boundingRect: viewportToScaled(boundingRect),
                          },
                          {
                            image: screenshot(boundingRect),
                          }
                        );
                      }}
                    />
                  );

                  return (
                    <Popup
                      popupContent={
                        <HighlightPopup
                          {...highlight}
                          onClick={() => {
                            this.removeHighlight(highlight.id);
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

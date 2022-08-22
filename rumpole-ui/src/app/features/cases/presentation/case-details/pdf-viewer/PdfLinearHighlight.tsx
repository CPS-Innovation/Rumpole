import React, { Component } from "react";
import { LTWHP } from "../../../../../../react-pdf-highlighter";

import classes from "./PdfLinearHighlight.module.scss";

interface Props {
  position: {
    boundingRect: LTWHP;
    rects: Array<LTWHP>;
  };
  onClick?: () => void;
  onMouseOver?: () => void;
  onMouseOut?: () => void;
  type: "search" | "redaction";
  isScrolledTo: boolean;
}

export class PdfLinearHighlight extends Component<Props> {
  render() {
    const { position, onClick, onMouseOver, onMouseOut, isScrolledTo, type } =
      this.props;

    const { rects } = position;

    return (
      <div
        className={`${classes["Highlight"]} ${
          isScrolledTo ? classes["Highlight--scrolledTo"] : ""
        }`}
      >
        <div className={classes["Highlight__parts"]}>
          {rects.map((rect, index) => (
            <div
              onMouseOver={onMouseOver}
              onMouseOut={onMouseOut}
              onClick={onClick}
              key={index}
              style={rect}
              className={classes[`Highlight__part__${type}`]}
            />
          ))}
        </div>
      </div>
    );
  }
}

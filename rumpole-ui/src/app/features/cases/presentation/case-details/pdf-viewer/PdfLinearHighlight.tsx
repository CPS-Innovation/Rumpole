import React from "react";
import { LTWHP } from "../../../../../../react-pdf-highlighter";

import classes from "./PdfLinearHighlight.module.scss";

interface Props {
  position: {
    boundingRect: LTWHP;
    rects: Array<LTWHP>;
  };

  type: "search" | "redaction";
  isScrolledTo: boolean;
}

export const PdfLinearHighlight: React.FC<Props> = ({
  position: { rects },
  isScrolledTo,
  type,
}) => (
  <div
    className={`${classes["Highlight"]} ${
      isScrolledTo ? classes["Highlight--scrolledTo"] : ""
    }`}
  >
    <div className={classes["Highlight__parts"]}>
      {rects.map((rect, index) => (
        <div
          key={index}
          style={rect}
          className={classes[`Highlight__part__${type}`]}
        />
      ))}
    </div>
  </div>
);

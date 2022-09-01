import React, { useEffect, useState } from "react";
import { LTWHP } from "../../../../../../react-pdf-highlighter";

import classes from "./PdfLinearHighlight.module.scss";
import { PdfLinearHighlightPartRedaction } from "./PdfLinearHighlightPartRedaction";
import { PdfLinearHighlightPartSearch } from "./PdfLinearHighlightPartSearch";

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
}) => {
  return (
    <div
      className={`${classes["Highlight"]} ${
        isScrolledTo ? classes["Highlight--scrolledTo"] : ""
      }`}
    >
      <div className={classes["Highlight__parts"]}>
        {rects.map((rect, index) =>
          type === "search" ? (
            <PdfLinearHighlightPartSearch key={index} rect={rect} />
          ) : (
            <PdfLinearHighlightPartRedaction key={index} rect={rect} />
          )
        )}
      </div>
    </div>
  );
};

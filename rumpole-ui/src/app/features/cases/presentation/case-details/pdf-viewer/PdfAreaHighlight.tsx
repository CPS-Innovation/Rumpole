import React from "react";
import { ViewportHighlight } from "../../../../../../react-pdf-highlighter";

import classes from "./PdfAreaHighlight.module.scss";

interface Props {
  position: ViewportHighlight["position"];

  isScrolledTo: boolean;
}

export const PdfAreaHighlight: React.FC<Props> = ({
  position,
  isScrolledTo,
  ...otherProps
}) => (
  <div
    className={`${classes["AreaHighlight"]} ${
      isScrolledTo ? classes["AreaHighlight--scrolledTo"] : ""
    }`}
  >
    <div
      className={classes["AreaHighlight__part"]}
      style={position.boundingRect}
      {...otherProps}
    ></div>
  </div>
);

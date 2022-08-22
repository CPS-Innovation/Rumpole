import React, { Component } from "react";
import { ViewportHighlight } from "../../../../../../react-pdf-highlighter";

import classes from "./PdfAreaHighlight.module.scss";

interface Props {
  position: ViewportHighlight["position"];
  type: "search" | "redaction";
  isScrolledTo: boolean;
}

// export class PdfAreaHighlight extends Component<Props> {
//   render() {
//     const { position, isScrolledTo, type, ...otherProps } = this.props;

//     return (
//       <div
//         className={`${classes["AreaHighlight"]} ${
//           isScrolledTo ? classes["AreaHighlight--scrolledTo"] : ""
//         }`}
//       >
//         <Rnd // todo: no need for Rnd
//           className={classes["AreaHighlight__part"]}
//           position={{
//             x: position.boundingRect.left,
//             y: position.boundingRect.top,
//           }}
//           size={{
//             width: position.boundingRect.width,
//             height: position.boundingRect.height,
//           }}
//           {...otherProps}
//         />
//       </div>
//     );
//   }
// }

export class PdfAreaHighlight extends Component<Props> {
  render() {
    const { position, isScrolledTo, type, ...otherProps } = this.props;

    return (
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
  }
}

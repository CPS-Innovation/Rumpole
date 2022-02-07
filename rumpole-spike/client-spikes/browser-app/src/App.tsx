import React, { useState } from "react";
import response from "./response.json";

function App() {
  const [lineIndex, setLineIndex] = useState(0);

  const { height, width, lines } = response.analyzeResult.readResults[0];

  const factor = 0.75;

  const docHeight = height * factor;
  const docWidth = width * factor;

  const line = lines[lineIndex];
  const [x0, y0, , , x2, y2] = line.boundingBox;
  const lineX = x0,
    lineY = y0,
    lineWidth = x2 - x0,
    lineHeight = y2 - y0;

  return (
    <>
      <div
        className="App"
        style={{ height: docHeight, width: docWidth, padding: 30 }}
      >
        <h1>Visual (Image)</h1>
        <button onClick={() => setLineIndex(lineIndex + 1)}>Next line</button>
        <svg
          xmlns="http://www.w3.org/2000/svg"
          viewBox={`0 0 ${width} ${height}`}
        >
          <rect x="0" y="0" width="100%" height="100%" />
          <image x="0" y="0" width="100%" height="100%" xlinkHref="image.png" />
          <rect
            x={lineX}
            y={lineY}
            width={lineWidth}
            height={lineHeight}
            fill="red"
            fillOpacity={0.5}
          ></rect>
        </svg>
      </div>

      <div style={{ padding: 30 }}>
        <h1>Screen Reader</h1>
        <code>
          {lines.map((line, index) => {
            return <div>{line.text}</div>;
          })}
        </code>
      </div>

      <div style={{ padding: 30 }}>
        <h1>PDF</h1>
        <embed
          style={{ height: docHeight, width: docWidth }}
          src="response2.pdf#toolbar=0"
        ></embed>
      </div>
    </>
  );
}

export default App;

import React, { useEffect, useState } from "react";
import "./App.css";
import results from "./Coordinator.json";
import analyseResult from "./AnalyzeResult.json";
type AnalyseResult = typeof analyseResult;

type PageSpec = {
  height: number;
  width: number;
  docHeight: number;
  docWidth: number;
  lineX: number;
  lineY: number;
  lineHeight: number;
  lineWidth: number;
  text: string[];
};

function App() {
  const lineIndex = 4;

  const [pdfDoc, setPdfDoc] = useState<string | undefined>();
  const [pngFile, setPngFile] = useState<string | undefined>();
  const [jsonFile, setJsonFile] = useState<string | undefined>();
  const [pageSpec, setPageSpec] = useState<PageSpec>();

  useEffect(() => {
    (async () => {
      if (!jsonFile) return;
      var result = await fetch(jsonFile);
      var content = (await result.json()) as AnalyseResult;

      const { height, width, lines } = content.analyzeResult.readResults[0];

      const factor = 0.5;

      const docHeight = height * factor;
      const docWidth = width * factor;

      const line = lines[lineIndex];
      const [x0, y0, , , x2, y2] = line.boundingBox;
      const lineX = x0,
        lineY = y0,
        lineWidth = x2 - x0,
        lineHeight = y2 - y0;

      setPageSpec({
        height,
        width,
        docHeight,
        docWidth,
        lineX,
        lineY,
        lineHeight,
        lineWidth,
        text: lines.map((line) => line.text),
      });
    })();
  }, [jsonFile]);

  const setPdf = (url: string) => {
    setPdfDoc(url);
    setPngFile(undefined);
    setJsonFile(undefined);
    setPageSpec(undefined);
  };

  const setPng = (url: string, jsonUrl: string) => {
    setPdfDoc(undefined);
    setJsonFile(jsonUrl);
    setPngFile(url);
  };

  return (
    <div className="App" style={{ display: "flex" }}>
      <div>
        <ul>
          {results.map((result) => (
            <li key={result.blobName} style={{ textAlign: "left" }}>
              {getFileName(result.pdfUrl)}
              <ul>
                <li>
                  <a target="_blank" href={result.pdfUrl}>
                    pdf in tab
                  </a>
                </li>
                <li>
                  <a href="#" onClick={() => setPdf(result.pdfUrl)}>
                    pdf in page
                  </a>
                </li>
                <li>
                  Image:{" "}
                  {result.pngUrls?.map((pngUrl, index) => (
                    <span key={index}>
                      {!!index && <>,&nbsp;</>}
                      <a
                        href="#"
                        onClick={() => setPng(pngUrl, result.jsonUrls[index])}
                      >
                        {index + 1}
                      </a>
                    </span>
                  ))}
                </li>
              </ul>
            </li>
          ))}
        </ul>
      </div>
      <div>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
      </div>
      <div>
        {pdfDoc && (
          <embed
            style={{ height: 1527, width: 1080 }}
            src={pdfDoc + "#toolbar=0"}
            type="application/pdf"
          ></embed>
        )}
      </div>

      {pageSpec && (
        <div
          className="App"
          style={{
            height: pageSpec.docHeight,
            width: pageSpec.docWidth,
            padding: 30,
          }}
        >
          <h1>Image</h1>
          {/* <button onClick={() => setLineIndex(lineIndex + 1)}>Next line</button> */}
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox={`0 0 ${pageSpec.width} ${pageSpec.height}`}
          >
            <rect x="0" y="0" width="100%" height="100%" />
            <image x="0" y="0" width="100%" height="100%" xlinkHref={pngFile} />
            <rect
              x={pageSpec.lineX}
              y={pageSpec.lineY}
              width={pageSpec.lineWidth}
              height={pageSpec.lineHeight}
              fill="red"
              fillOpacity={0.5}
            ></rect>
          </svg>
          <code style={{ textAlign: "left" }}>
            <br />
            <br />
            <br />
            <br />
            <hr />
            <h1>Raw Text</h1>
            {pageSpec.text.map((text) => (
              <div>{text}</div>
            ))}
          </code>
        </div>
      )}
    </div>
  );
}

function getFileName(url: string) {
  return decodeURIComponent(url.split("/").pop()?.split("?")[0] || "");
}

export default App;

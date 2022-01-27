import React from "react";
import { useTracker } from "./app/useTracker";

const urlSearchParams = new URLSearchParams(window.location.search);
const params = Object.fromEntries(urlSearchParams.entries());

function App() {
  const { ticks, tracker } = useTracker(params["caseId"]);

  return (
    <div style={{ fontFamily: "courier", fontSize: 20, display: "flex" }}>
      <div style={{ wordWrap: "break-word", width: 1205 }}>
        {Array.from(Array(ticks).keys()).map((i) => (i % 10 ? "." : "|"))}
      </div>
      <div>&nbsp;</div>

      {tracker && (
        <code>
          <div>Is complete: {tracker.isIndexed ? "x" : "o"}</div>
          <div>
            {tracker.documents.map((document) => (
              <div>
                Doc id: {document.documentId} Pdf?:{" "}
                {document.pdfUrl ? "x" : "o"} Pngs?:{" "}
                {document.pngDetails?.every((item) => item.url) ? "x" : "o"}{" "}
                Searched?:{" "}
                {document.pngDetails?.every((item) => item.dimensions)
                  ? "x"
                  : "o"}
              </div>
            ))}
          </div>
        </code>
      )}
    </div>
  );
}

export default App;

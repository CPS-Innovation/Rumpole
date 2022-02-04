import React from "react";
import { Search } from "./app/Search";
import { useTracker } from "./app/useTracker";

const urlSearchParams = new URLSearchParams(window.location.search);
const params = Object.fromEntries(urlSearchParams.entries());

function App() {
  const { ticks, tracker } = useTracker(params["caseId"]);

  const no = () => <span style={{ color: "red" }}>&#x25A1;</span>;

  const yes = () => <span style={{ color: "green" }}>&#x2588;</span>;

  return (
    <div
      style={{
        fontFamily: "courier",
        fontSize: 20,
        padding: 30,
        display: "flex",
      }}
    >
      <div>
        <div
          style={{
            wordWrap: "break-word",
            width: 600,
            fontSize: 40,
          }}
        >
          {/* {Array.from(Array(ticks).keys()).map((i) => (i % 10 ? "." : "|"))} */}
          {ticks / 10}
        </div>

        {tracker && (
          <>
            <code>
              <div>
                {tracker.documents.map((document) => (
                  <div key={document.documentId}>
                    Doc id: {document.documentId} Pdf?:{" "}
                    {document.pdfUrl ? yes() : no()} Pngs?:{" "}
                    {document.pngDetails?.every((item) => item.url)
                      ? yes()
                      : no()}
                    &nbsp; &nbsp; &nbsp;&nbsp; Analyzed?:
                    {document.pngDetails?.every((item) => item.dimensions)
                      ? yes()
                      : no()}
                  </div>
                ))}
              </div>
              <div>
                Is indexed and complete?: {tracker.isIndexed ? yes() : no()}
              </div>
            </code>
            <br />

            <div>{tracker.isIndexed && <Search />}</div>
          </>
        )}
      </div>

      <div
        style={{
          backgroundColor: "#eeeeee",
          fontSize: 10,
          padding: 10,
          flexGrow: 1,
        }}
      >
        <pre>{JSON.stringify(tracker, null, 2)}</pre>
      </div>
    </div>
  );
}

export default App;

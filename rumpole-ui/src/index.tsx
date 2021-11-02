import React from "react";
import ReactDOM from "react-dom";
import { App } from "./app/App";
import reportWebVitals from "./reportWebVitals";

// todo: use a REACT_APP_* flag to allow more granular control of which builds/contexts to use mocking
if (process.env.NODE_ENV === "development") {
  const { worker } = require("./mock-api/browser");
  worker.start();
}

ReactDOM.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
  document.getElementById("root")
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();

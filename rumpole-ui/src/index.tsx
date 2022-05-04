import React from "react";
import ReactDOM from "react-dom";
import { App } from "./app/App";
import { SetupMockApi } from "./mock-api/browser";
import reportWebVitals from "./reportWebVitals";
import "./styles.scss";

if (process.env.REACT_APP_MOCK_API_SOURCE === "dev") {
  // only require if we need to, this way production bundles don't contain the mock api code
  const { setupMockApi } = require("./mock-api/browser") as {
    setupMockApi: SetupMockApi;
  };

  setupMockApi({
    baseUrl: process.env.REACT_APP_GATEWAY_BASE_URL!,
    maxDelay: Number(process.env.REACT_APP_MOCK_API_MAX_DELAY!),
    sourceName: "dev",
  });
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

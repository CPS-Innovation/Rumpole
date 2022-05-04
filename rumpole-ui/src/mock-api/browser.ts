// src/mocks/browser.js
import { setupWorker } from "msw";
import { setupHandlers } from "./handlers";
import { MockApiConfig } from "./MockApiConfig";

export type SetupMockApi = typeof setupMockApi;

export const setupMockApi = async (config: MockApiConfig) => {
  if ((window as any).msw) {
    return;
  }

  const worker = setupWorker(...setupHandlers(config));
  await worker.start();

  (window as any).msw = { worker }; // attach to window for cypress testing purposes
};

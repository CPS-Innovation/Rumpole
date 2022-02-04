import { useEffect, useState } from "react";
import { Tracker } from "./tracker";

const COORDNINATOR_URL = process.env.REACT_APP_COORDINATOR!;
const TRACKER_URL = process.env.REACT_APP_TRACKER!;

const call = async () => {
  var response = await fetch(TRACKER_URL);
  return await response.json();
};

const resolveHttps = (url: string) =>
  COORDNINATOR_URL.startsWith("https://")
    ? url.replace("http://", "https://")
    : url;

export const useTracker = (caseId: string | null) => {
  const [ticks, setTicks] = useState<number>(0);
  const [trackerUrl, setTrackerUrl] = useState<string>();

  const [inProgressPollCall, setInProgressPollCall] = useState(false);
  const [tracker, setTracker] = useState<Tracker>();

  useEffect(() => {
    (async () => {
      const koResponse = await fetch(COORDNINATOR_URL);
      const koResponseContent = await koResponse.json();
      const statusQueryCallResponse = await fetch(
        resolveHttps(koResponseContent.statusQueryGetUri as string)
      );

      const statusQueryContent = await statusQueryCallResponse.json();

      setTrackerUrl(statusQueryContent.input.TrackerUrl);
    })();
  }, []);

  useEffect(() => {
    if (!trackerUrl) return;

    const interval = setInterval(async () => {
      setTicks(ticks + 1);
      if (ticks % 10 === 0 && !inProgressPollCall) {
        setInProgressPollCall(true);
        const response = await call();
        setInProgressPollCall(false);
        setTracker(response);
      }
    }, 100);

    return () => {
      clearInterval(interval);
    };
  });

  return { ticks, tracker };
};

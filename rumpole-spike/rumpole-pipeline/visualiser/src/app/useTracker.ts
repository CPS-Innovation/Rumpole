import { useEffect, useState } from "react";
import { Tracker } from "./tracker";

const call = async () => {
  var response = await fetch(
    "http://localhost:7071/api/cases/123/tracker?code=foo"
  );
  return await response.json();
};

export const useTracker = (caseId: string | null) => {
  const [ticks, setTicks] = useState<number>(0);
  const [trackerUrl, setTrackerUrl] = useState<string>();

  const [inProgressPollCall, setInProgressPollCall] = useState(false);
  const [tracker, setTracker] = useState<Tracker>();

  useEffect(() => {
    (async () => {
      const koResponse = await fetch(
        "http://localhost:7071/api/cases/123?code=foo"
      );
      const koResponseContent = await koResponse.json();
      const statusQueryCallResponse = await fetch(
        koResponseContent.statusQueryGetUri
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

import { useEffect, useState } from "react";
import results from "./analyze-result-inches.json";
import pixelDimensions from "./dimensions-pixels.json";

const round = (num: number) => Math.round(num * 100) / 100;

export const Inches: React.FC = () => {
  const [index, setIndex] = useState<number>(0);
  const [pdfSrc, setPdfSrc] = useState<string>(
    `/101-after.pdf#toolbar=0&page=1`
  );
  const pageResult = results.readResults[0];
  const allWords = pageResult.lines; //.flatMap((line) => line.words);

  useEffect(() => {
    const id = setInterval(() => setIndex(index + 1), 600);
    return () => clearInterval(id);
  });

  const word = allWords[index];

  const ratioHeight = round(pixelDimensions.Height / pageResult.height);
  const ratioWidth = round(pixelDimensions.Width / pageResult.width);

  const boxInches = word.boundingBox;
  const boxPixels = word.boundingBox.map((inch) => round(inch * ratioHeight));

  const [x0, y0, , , x2, y2] = boxPixels;
  const lineX = x0,
    lineY = y0,
    lineWidth = x2 - x0,
    lineHeight = y2 - y0;

  const handleClick = () => {
    setPdfSrc(`/101-after.pdf#toolbar=0&page=2`);
  };

  return (
    <div>
      Dimensions: {index}
      <br />
      {pageResult.height} / {pageResult.width} inches <br />
      {pixelDimensions.Height} / {pixelDimensions.Width} pixels <br />
      {ratioHeight} / {ratioWidth} ratios <br />
      Word: <br />
      <b>{word.text}</b> <br />
      Points: <br />
      {boxInches.join(", ")} inches <br />
      {boxPixels.join(", ")} pixels
      <hr />
      <div
        className="App"
        style={{
          height: pixelDimensions.Height,
          width: pixelDimensions.Width,
          padding: 30,
        }}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          viewBox={`0 0 ${pixelDimensions.Width} ${pixelDimensions.Height}`}
        >
          <rect x="0" y="0" width="100%" height="100%" />
          <image x="0" y="0" width="100%" height="100%" xlinkHref="/0.png" />
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
      <button onClick={handleClick}>Next page</button>
      <hr />
      <embed
        key={pdfSrc}
        style={{ height: 1527 / 1.2, width: 1080 / 1.2 }}
        src={pdfSrc}
        type="application/pdf"
      ></embed>
    </div>
  );
};

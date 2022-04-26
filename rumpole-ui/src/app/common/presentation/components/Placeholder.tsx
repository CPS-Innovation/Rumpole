type Props = {
  height: number;
  width?: number;
  marginTop?: number;
  marginBottom?: number;
  backgroundColor?: string;
};

export const Placeholder: React.FC<Props> = ({
  height,
  width,
  marginTop = 5,
  marginBottom = 5,
  backgroundColor = "#eeeeee",
}) => {
  return (
    <div
      style={{
        height: height - 2,
        marginTop,
        marginBottom,
        backgroundColor,
        border: "1px dashed #888888",
        width,
      }}
    ></div>
  );
};

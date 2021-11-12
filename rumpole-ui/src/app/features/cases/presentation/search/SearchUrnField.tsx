import { FC, KeyboardEvent } from "react";

type UrnFieldProps = {
  value: string;
  onChange: (val: string) => void;
  onSubmit: () => void;
};

export const SearchUrnField: FC<UrnFieldProps> = ({
  value,
  onChange,
  onSubmit,
}) => {
  const handleKeyPress = (event: KeyboardEvent<HTMLInputElement>) =>
    event.key === "Enter" && onSubmit();

  return (
    <input
      data-testid="input-search-urn"
      style={{
        flexGrow: 1,
        height: "100%",
        padding: 15,
        border: "none",
        outline: "none",
        fontSize: 26,
      }}
      autoFocus
      value={value}
      onChange={(event) => onChange(event.target.value)}
      onKeyPress={(event) => handleKeyPress(event)}
      placeholder="Enter a URN"
    />
  );
};

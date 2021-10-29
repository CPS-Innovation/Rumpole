import { FC, KeyboardEvent } from "react";

type UrnFieldProps = {
  value: string;
  onChange: (val: string) => void;
  onSubmit: () => void;
};

export const UrnField: FC<UrnFieldProps> = ({ value, onChange, onSubmit }) => {
  const handleKeyPress = (event: KeyboardEvent<HTMLInputElement>) =>
    event.key === "Enter" && onSubmit();

  return (
    <input
      style={{
        flexGrow: 1,
        height: "100%",
        padding: 15,
        border: "none",
        outline: "none",
        fontSize: 24,
      }}
      autoFocus
      value={value}
      onChange={(event) => onChange(event.target.value)}
      onKeyPress={(event) => handleKeyPress(event)}
    />
  );
};

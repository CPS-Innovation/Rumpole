import { FC, KeyboardEvent } from "react";

type UrnFieldProps = {
  value: string;
  onChange: (val: string) => void;
  onSubmit: () => void;
};

export const SearchInput: FC<UrnFieldProps> = ({
  value,
  onChange,
  onSubmit,
}) => {
  const handleKeyPress = (event: KeyboardEvent<HTMLInputElement>) =>
    event.key === "Enter" && onSubmit();

  return (
    <input
      data-testid="input-search-urn"
      autoFocus
      value={value}
      onChange={(event) => onChange(event.target.value)}
      onKeyPress={(event) => handleKeyPress(event)}
    />
  );
};

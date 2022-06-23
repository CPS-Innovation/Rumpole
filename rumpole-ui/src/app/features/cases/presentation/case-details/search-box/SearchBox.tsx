import { KeyboardEvent } from "react";
import { Input } from "../../../../../common/presentation/components";
import classes from "./SearchBox.module.scss";

type Props = {
  value: undefined | string;
  handleChange: (val: string) => void;
  handleSubmit: () => void;
};

export const SearchBox: React.FC<Props> = ({
  value,
  handleChange,
  handleSubmit,
}) => {
  const handleKeyPress = (event: KeyboardEvent<HTMLInputElement>) =>
    event.key === "Enter" && handleSubmit();

  return (
    <div className={classes.container}>
      <Input
        value={value}
        data-testid="input-search-case"
        onChange={handleChange}
        onKeyDown={handleKeyPress}
        label={{ children: "Search", className: "govuk-label--s" }}
        suffix={{
          children: (
            <button
              data-testid="btn-search-case"
              className={classes.button}
              type="submit"
              onClick={handleSubmit}
            ></button>
          ),
          className: classes.suffix,
        }}
      />
    </div>
  );
};

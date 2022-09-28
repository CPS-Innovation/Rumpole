import { KeyboardEvent, useCallback } from "react";
import { Input } from "../../../../../common/presentation/components";
import classes from "./SearchBox.module.scss";

type Props = {
  labelText: string;
  value: undefined | string;
  handleChange: (val: string) => void;
  handleSubmit: () => void;
  "data-testid"?: string;
};

export const SearchBox: React.FC<Props> = ({
  value,
  handleChange,
  handleSubmit,
  labelText,
  "data-testid": dataTestId,
}) => {
  const localHandleKeyDown = (event: KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      handleSubmit();
    }
  };

  const localHandleChange = useCallback(
    (val) => {
      handleChange(val.replace(/\s/g, ""));
    },
    [handleChange]
  );

  return (
    <div className={classes.container}>
      <Input
        autoFocus
        data-testid={dataTestId && `input-${dataTestId}`}
        value={value}
        onChange={localHandleChange}
        onKeyDown={localHandleKeyDown}
        label={{ children: labelText, className: "govuk-label--s" }}
        suffix={{
          children: (
            <button
              data-testid={dataTestId && `btn-${dataTestId}`}
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

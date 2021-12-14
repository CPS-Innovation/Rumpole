import * as GDS from "govuk-react-jsx";

type InputProps = Omit<
  React.DetailedHTMLProps<
    React.InputHTMLAttributes<HTMLInputElement>,
    HTMLInputElement
  >,
  "onChange"
> & {
  className?: string;
  describedBy?: string;
  errorMessage?: string;
  formGroup?: { className: string };
  hint?: string;
  label?: string;
  name?: string;
  id?: string;
  prefix?: string;
  suffix?: string;
  // as a convenience, let consumer just deal with the event value rather than the event
  onChange?: (val: string) => void;
};

export const Input: React.FC<InputProps> = ({ onChange, ...props }) => {
  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) =>
    onChange && onChange(event.target.value);

  return <GDS.Input {...props} onChange={handleChange}></GDS.Input>;
};

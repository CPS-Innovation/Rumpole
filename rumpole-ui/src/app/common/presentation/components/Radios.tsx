import * as GDS from "govuk-react-jsx";

type RadiosProps = {
  value: string | undefined;
  name: string;
  items: {
    key?: string; //reactListKey
    value: string | undefined;
    text: React.ReactNode; // children
  }[];
  onChange: (value: string | undefined) => void;
};

export const Radios: React.FC<RadiosProps> = ({
  items,
  onChange: propOnChange,
  ...props
}) => {
  const processedItems = items.map((item) => ({
    ...item,
    reactListKey: item.key || item.value,
    children: item.text,
  }));

  const onChange: React.ChangeEventHandler<HTMLInputElement> = (event) =>
    propOnChange(event.target.value);

  return (
    <GDS.Radios
      items={processedItems}
      onChange={onChange}
      {...props}
      className="govuk-radios--small"
    />
  );
};

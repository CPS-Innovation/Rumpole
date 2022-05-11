import * as GDS from "govuk-react-jsx";

type PanelProps = React.DetailedHTMLProps<
  React.LabelHTMLAttributes<HTMLDivElement>,
  HTMLDivElement
>;

type ItemProps = React.DetailedHTMLProps<
  React.AnchorHTMLAttributes<HTMLAnchorElement>,
  HTMLAnchorElement
> & {
  id: string;
  label: string;
  panel: PanelProps;
};

type TabsProps = React.DetailedHTMLProps<
  React.LabelHTMLAttributes<HTMLDivElement>,
  HTMLDivElement
> & {
  idPrefix: string;
  title: string;
  items: ItemProps[];
};

export const Tabs: React.FC<TabsProps> = (props) => {
  return <GDS.Tabs {...props} />;
};

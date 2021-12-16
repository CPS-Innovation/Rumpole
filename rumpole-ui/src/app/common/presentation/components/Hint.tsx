import * as GDS from "govuk-react-jsx";
import styles from "./Hint.module.scss";

type HintProps = {
  className?: string;
  attributes?: React.DetailedHTMLProps<
    React.HTMLAttributes<HTMLDivElement>,
    HTMLDivElement
  >;
};

export const Hint: React.FC<HintProps> = ({
  className,
  attributes,
  children,
}) => {
  return (
    <GDS.Hint className={`${styles.root} ${className}`} {...attributes}>
      {children}
    </GDS.Hint>
  );
};

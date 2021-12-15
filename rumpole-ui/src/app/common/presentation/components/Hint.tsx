import * as GDS from "govuk-react-jsx";
import styles from "./Hint.module.scss";

export const Hint: React.FC = () => {
  return (
    <GDS.Hint className={styles.root}>
      Search and review a CPS case in England
    </GDS.Hint>
  );
};

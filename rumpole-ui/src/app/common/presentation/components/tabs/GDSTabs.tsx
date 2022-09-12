/* istanbul ignore file */
import * as GDS from "govuk-react-jsx";
import { CommonTabsProps } from "./types";

/* istanbul ignore next */
export const GDSTabs: React.FC<CommonTabsProps> = (props) => {
  return <GDS.Tabs {...props} />;
};

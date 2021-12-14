import * as GDS from "govuk-react-jsx";

export const Label: React.FC = ({ children }) => {
  return <GDS.Label className="govuk-label--s">{children}</GDS.Label>;
};

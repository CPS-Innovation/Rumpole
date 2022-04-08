import React from "react";
import * as GDS from "govuk-react-jsx";

type BackLinkProps = {
  to: string;
};

export const BackLink: React.FC<BackLinkProps> = (props) => {
  return <GDS.BackLink data-testid="link-back-link" {...props} />;
};

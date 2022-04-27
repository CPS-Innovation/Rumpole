import * as GDS from "govuk-react-jsx";
import React from "react";
import { PageContentWrapper } from "./PageContentWrapper";

type ErrorSummaryProps = React.DetailedHTMLProps<
  React.HTMLAttributes<HTMLDivElement>,
  HTMLDivElement
> & {
  className?: string;
  titleChildren?: string;
  descriptionChildren?: React.ReactNode;
  errorList?: React.DetailedHTMLProps<
    React.HTMLAttributes<HTMLAnchorElement>,
    HTMLAnchorElement
  > & {
    reactListKey: string;
  };
};

export const ErrorSummary: React.FC<ErrorSummaryProps> = (props) => {
  const resolvedProps = {
    ...props,
    descriptionChildren: React.isValidElement(props.descriptionChildren)
      ? props.descriptionChildren
      : props.descriptionChildren?.toString(),
  };

  return (
    <PageContentWrapper>
      <GDS.ErrorSummary {...resolvedProps} />
    </PageContentWrapper>
  );
};

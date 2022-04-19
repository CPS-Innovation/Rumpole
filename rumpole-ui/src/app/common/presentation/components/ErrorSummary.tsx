import * as GDS from "govuk-react-jsx";
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
  return (
    <PageContentWrapper>
      <GDS.ErrorSummary {...props} />
    </PageContentWrapper>
  );
};

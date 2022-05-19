import { Component, ErrorInfo, ReactNode } from "react";
import { Layout } from "../layout/Layout";
import { PageContentWrapper } from "./PageContentWrapper";

interface Props {
  children: ReactNode;
}

interface State {
  hasError: boolean;
  error: Error | undefined;
}

class ErrorBoundary extends Component<Props, State> {
  public state: State = {
    hasError: false,
    error: undefined,
  };

  public static getDerivedStateFromError(error: Error): State {
    // Update state so the next render will show the fallback UI.
    return { hasError: true, error };
  }

  public componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error("Uncaught error:", error, errorInfo);
  }

  public render() {
    if (this.state.hasError) {
      return (
        <Layout>
          <PageContentWrapper>
            <h1
              className="govuk-heading-xl"
              data-testid="txt-error-page-heading"
            >
              Sorry, there is a problem with the service
            </h1>
            <p className="govuk-body">
              Try again later, or{" "}
              <a href="/" className="govuk-link">
                click here to start a new search
              </a>
              .
            </p>
            <div className="govuk-inset-text">
              {this.state.error?.toString()}
            </div>
          </PageContentWrapper>
        </Layout>
      );
    }

    return this.props.children;
  }
}

export default ErrorBoundary;

import React, { Component, ErrorInfo, ReactNode } from "react";
import { Layout } from "../layout/Layout";
import { PageContentWrapper } from "./PageContentWrapper";

interface Props {
  children: ReactNode;
}

interface State {
  hasError: boolean;
}

class ErrorBoundary extends Component<Props, State> {
  public state: State = {
    hasError: false,
  };

  public static getDerivedStateFromError(_: Error): State {
    // Update state so the next render will show the fallback UI.
    return { hasError: true };
  }

  public componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error("Uncaught error:", error, errorInfo);
  }

  public render() {
    if (this.state.hasError) {
      return (
        <Layout>
          <PageContentWrapper>
            <h1 className="govuk-heading-xl">
              Sorry... an unexpected error has occurred
            </h1>
          </PageContentWrapper>
        </Layout>
      );
    }

    return this.props.children;
  }
}

export default ErrorBoundary;

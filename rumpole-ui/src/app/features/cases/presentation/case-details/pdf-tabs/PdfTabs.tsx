import { Tabs } from "../../../../../common/presentation/components/Tabs";
import { CaseDocumentWithUrl } from "../../../domain/CaseDocumentWithUrl";
import classes from "../index.module.scss";
import { PdfTab } from "./PdfTab";

type PdfTabsProps = {
  tabsState: {
    items: CaseDocumentWithUrl[];
    authToken: undefined | string;
  };
};

export const PdfTabs: React.FC<PdfTabsProps> = ({
  tabsState: { items, authToken },
}) => {
  // Important to only render Tabs when there is at least one tab.  If we first render Tabs component
  //  with no tabs, the underlying govuk JS quits its initialisation method, due to there being
  //  no tabs.  Subsequently adding tabs doesn't fully work if the initialisation fails.
  if (!items.length) {
    return null;
  }

  return (
    <Tabs
      className={classes.pdfTabs}
      idPrefix="pdf"
      items={items.map((item) => ({
        id: item.tabSafeId,
        label: item.fileName,
        panel: {
          children: <PdfTab caseDocument={item} authToken={authToken} />,
        },
      }))}
      title="Contents"
    />
  );
};

import { Tabs } from "../../../../../common/presentation/components/Tabs";
import { CaseDocument } from "../../../domain/CaseDocument";
import classes from "../index.module.scss";
import { Empty } from "./Empty";
import { PdfTab } from "./PdfTab";

type PdfTabsProps = {
  items: CaseDocument[];
};

export const PdfTabs: React.FC<PdfTabsProps> = ({ items }) => {
  // Important to only render Tabs when there is at least one tab.  If we first render Tabs component
  //  with no tabs, the underlying govuk JS quits its initialisation method, due to there being
  //  no tabs.  Subsequently adding tabs doesn't fully work if the initialisation fails.
  return items.length ? (
    <Tabs
      className={classes.pdfTabs}
      idPrefix="pdf"
      items={items.map((item) => ({
        id: item.fileName,
        label: item.fileName,
        panel: {
          children: [<PdfTab key={item.documentId} caseDocument={item} />],
        },
      }))}
      title="Contents"
    />
  ) : (
    <Empty />
  );
};

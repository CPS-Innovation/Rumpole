import { Tabs } from "../../../../../common/presentation/components/tabs";
import { CaseDocumentWithUrl } from "../../../domain/CaseDocumentWithUrl";

import { PdfTab } from "./PdfTab";

type PdfTabsProps = {
  tabsState: {
    items: CaseDocumentWithUrl[];
    authToken: undefined | string;
  };
  handleClosePdf: (caseDocument: { tabSafeId: string }) => void;
};

export const PdfTabs: React.FC<PdfTabsProps> = ({
  tabsState: { items, authToken },
  handleClosePdf,
}) => {
  return (
    <Tabs
      idPrefix="pdf"
      items={items.map((item) => ({
        id: item.tabSafeId,
        label: item.fileName,
        panel: {
          children: <PdfTab caseDocument={item} authToken={authToken} />,
        },
      }))}
      title="Contents"
      handleClosePdf={handleClosePdf}
    />
  );
};

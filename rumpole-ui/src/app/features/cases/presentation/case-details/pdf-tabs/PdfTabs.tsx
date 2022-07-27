import { Tabs } from "../../../../../common/presentation/components/tabs";
import { CaseDocumentViewModel } from "../../../domain/CaseDocumentViewModel";

import { PdfTab } from "./PdfTab";

type PdfTabsProps = {
  tabsState: {
    items: CaseDocumentViewModel[];
    authToken: undefined | string;
  };
  handleClosePdf: (caseDocument: { tabSafeId: string }) => void;
  handleLaunchSearchResults: () => void;
};

export const PdfTabs: React.FC<PdfTabsProps> = ({
  tabsState: { items, authToken },
  handleClosePdf,
  handleLaunchSearchResults,
}) => {
  return (
    <Tabs
      idPrefix="pdf"
      items={items.map((item) => ({
        id: item.tabSafeId,
        label: item.fileName,
        panel: {
          children: (
            <PdfTab
              caseDocumentViewModel={item}
              authToken={authToken}
              handleLaunchSearchResults={handleLaunchSearchResults}
            />
          ),
        },
      }))}
      title="Contents"
      handleClosePdf={handleClosePdf}
    />
  );
};

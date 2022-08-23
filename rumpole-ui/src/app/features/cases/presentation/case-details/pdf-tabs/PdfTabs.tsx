import { Tabs } from "../../../../../common/presentation/components/tabs";
import { CaseDocumentViewModel } from "../../../domain/CaseDocumentViewModel";
import { CaseDetailsState } from "../../../hooks/use-case-details-state/useCaseDetailsState";

import { PdfTab } from "./PdfTab";

type PdfTabsProps = {
  tabsState: {
    items: CaseDocumentViewModel[];
    authToken: undefined | string;
  };
  handleClosePdf: (caseDocument: { tabSafeId: string }) => void;
  handleLaunchSearchResults: () => void;
  handleAddRedaction: CaseDetailsState["handleAddRedaction"];
  handleRemoveRedaction: CaseDetailsState["handleRemoveRedaction"];
};

export const PdfTabs: React.FC<PdfTabsProps> = ({
  tabsState: { items, authToken },
  handleClosePdf,
  handleLaunchSearchResults,
  handleAddRedaction,
  handleRemoveRedaction,
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
              handleAddRedaction={handleAddRedaction}
              handleRemoveRedaction={handleRemoveRedaction}
            />
          ),
        },
      }))}
      title="Contents"
      handleClosePdf={handleClosePdf}
    />
  );
};

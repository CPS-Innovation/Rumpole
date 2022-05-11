import { Placeholder } from "../../../../../common/presentation/components/Placeholder";
import { GATEWAY_BASE_URL } from "../../../../../config";
import { CaseDocument } from "../../../domain/CaseDocument";
import PdfViewer from "../pdf-viewer/PdfViewer";

type PdfTabProps = {
  caseDocument: CaseDocument;
};

export const PdfTab: React.FC<PdfTabProps> = ({
  caseDocument: { documentId, fileName },
}) => {
  const url = `${GATEWAY_BASE_URL}/api/documents/${documentId}/${fileName}`;

  return (
    <>
      <Placeholder
        height={50}
        marginTop={-31}
        marginLeft={-21}
        marginRight={-21}
      />
      <PdfViewer url={url} />
    </>
  );
};

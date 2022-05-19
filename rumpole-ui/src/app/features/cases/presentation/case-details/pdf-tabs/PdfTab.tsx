import { Placeholder } from "../../../../../common/presentation/components/Placeholder";
import { CaseDocumentWithUrl } from "../../../domain/CaseDocumentWithUrl";
import PdfViewer from "../pdf-viewer/PdfViewer";
import { Wait } from "../pdf-viewer/Wait";

type PdfTabProps = {
  caseDocument: CaseDocumentWithUrl;
  authToken: string | undefined;
};

export const PdfTab: React.FC<PdfTabProps> = ({
  caseDocument: { url },
  authToken,
}) => {
  return (
    <>
      <Placeholder
        height={50}
        marginTop={-31}
        marginLeft={-21}
        marginRight={-21}
      />
      {url && authToken ? (
        <PdfViewer url={url} authToken={authToken} />
      ) : (
        <Wait />
      )}
    </>
  );
};

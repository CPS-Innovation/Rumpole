import {
  CommonDateTimeFormats,
  formatDate,
} from "../../../../../common/utils/dates";
import { CaseDocument } from "../../../domain/CaseDocument";

import classes from "./Accordion.module.scss";

type Props = {
  caseDocument: CaseDocument;
  handleOpenDocument: (caseDocument: CaseDocument) => void;
};

export const AccordionDocument: React.FC<Props> = ({
  caseDocument,
  handleOpenDocument,
}) => {
  return (
    <tr className="govuk-table__row">
      <td
        className="govuk-table__cell govuk-body-s openMe"
        style={{ wordWrap: "break-word" }}
      >
        <a
          href={`#${caseDocument.fileName}`}
          onClick={(ev) => {
            handleOpenDocument(caseDocument);
            //ev.preventDefault();
          }}
          data-testid={`link-document-${caseDocument.documentId}`}
        >
          {caseDocument.fileName}
        </a>
      </td>
      <td className={`govuk-table__cell govuk-body-s ${classes.date}`}>
        {caseDocument.createdDate &&
          formatDate(
            caseDocument.createdDate,
            CommonDateTimeFormats.ShortDateTextMonth
          )}
      </td>
    </tr>
  );
};

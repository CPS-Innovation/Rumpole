import {
  CommonDateTimeFormats,
  formatDate,
} from "../../../../../common/utils/dates";

import classes from "./Accordion.module.scss";

type Props = { docLabel: string; docDate: string };

export const AccordionDocument: React.FC<Props> = ({ docLabel, docDate }) => {
  return (
    <tr className="govuk-table__row">
      <td
        className="govuk-table__cell govuk-body-s openMe"
        style={{ wordWrap: "break-word" }}
      >
        <a href="">{docLabel}</a>
      </td>
<<<<<<< HEAD
      <td className={`govuk-table__cell govuk-body-s ${classes.date}`}>
        {docDate &&
          formatDate(docDate, CommonDateTimeFormats.ShortDateTextMonth)}
=======
      <td className="govuk-table__cell govuk-body-s dateWidthCPS">
        {docDate && formatDate(docDate, CommonDateTimeFormats.ShortDateTextMonth)}
>>>>>>> main
      </td>
    </tr>
  );
};

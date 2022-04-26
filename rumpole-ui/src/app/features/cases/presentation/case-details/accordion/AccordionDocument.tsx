import {
  CommonDateTimeFormats,
  formatDate,
} from "../../../../../common/utils/dates";

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
      <td className="govuk-table__cell govuk-body-s dateWidthCPS">
        {formatDate(docDate, CommonDateTimeFormats.ShortDateTextMonth)}
      </td>
    </tr>
  );
};

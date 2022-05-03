import { AccordionDocument } from "./AccordionDocument";
import classes from "./Accordion.module.scss";
import { AccordionNoDocuments } from "./AccordionNoDocuments";

type Props = {
  sectionId: string;
  sectionLabel: string;
  docs: { docId: string; docLabel: string; docDate: string }[];
  isOpen: boolean;
  handleToggleOpenSection: (id: string) => void;
};

export const AccordionSection: React.FC<Props> = ({
  sectionId,
  sectionLabel,
  docs,
  isOpen,
  handleToggleOpenSection,
}) => {
  return (
    <div className={`${classes["accordion-section"]}`} aria-expanded={isOpen}>
      <div
        className={`${classes["accordion-section-header"]}`}
        role="button"
        tabIndex={0}
        onClick={() => handleToggleOpenSection(sectionId)}
      >
        <h2 className="govuk-heading-s">{sectionLabel}</h2>
        <span className={`${classes["icon"]}`}></span>
      </div>
      <div className={`${classes["accordion-section-body"]}`}>
        <table className="govuk-table">
          {!docs.length ? (
            <tbody>
              <AccordionNoDocuments />
            </tbody>
          ) : (
            <>
              <thead>
                <tr className="govuk-table__row">
                  <th scope="col" className="govuk-table__header"></th>
                  <th
                    scope="col"
                    className="govuk-table__header govuk-body-s"
                    style={{ fontWeight: 400 }}
                  >
                    Date added
                  </th>
                </tr>
              </thead>
              <tbody>
                {docs.map(({ docId, docLabel, docDate }) => (
                  <AccordionDocument
                    key={docId}
                    docLabel={docLabel}
                    docDate={docDate}
                  />
                ))}
              </tbody>
            </>
          )}
        </table>
      </div>
    </div>
  );
};
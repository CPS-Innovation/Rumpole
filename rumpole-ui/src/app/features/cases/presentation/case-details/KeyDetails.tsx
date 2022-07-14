import { Placeholder } from "../../../../common/presentation/components";
import { CaseDetails } from "../../domain/CaseDetails";
import classes from "./index.module.scss";

export const KeyDetails: React.FC<{ caseDetails: CaseDetails }> = ({
  caseDetails,
}) => {
  return (
    <div>
      <h1
        className={`govuk-heading-m ${classes.defendantName}`}
        data-testid="txt-defendant-name"
      >
        {caseDetails.leadDefendant.surname},{" "}
        {caseDetails.leadDefendant.firstNames}
      </h1>
      <span className="govuk-caption-m" data-testid="txt-case-urn">
        {caseDetails.uniqueReferenceNumber}
      </span>
      <span className="govuk-caption-m" data-testid="txt-dob">
        <Placeholder height={30} marginTop={0} marginBottom={0} />
      </span>
    </div>
  );
};

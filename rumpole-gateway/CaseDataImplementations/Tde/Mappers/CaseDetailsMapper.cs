using System.Linq;
using BusinessDomain = RumpoleGateway.Domain.CaseData;
using ApiDomain = RumpoleGateway.CaseDataImplementations.Tde.Domain;

namespace RumpoleGateway.CaseDataImplementations.Tde.Mappers
{
    public class CaseDetailsMapper : ICaseDetailsMapper
    {
        public BusinessDomain.CaseDetails MapCaseDetails(ApiDomain.CaseDetails caseDetails)
        {
            var summary = caseDetails.Summary;
            var defendants = caseDetails.Defendants;
            var preChargeDecisionRequests = caseDetails.PreChargeDecisionRequests;

            var leadDefendant = GetLeadDefendantDetails(caseDetails);

            return new BusinessDomain.CaseDetails
            {
                Id = summary.Id,
                UniqueReferenceNumber = summary.Urn,
                LeadDefendantDetails = MapDefendant(leadDefendant),
            };
        }

        private BusinessDomain.DefendantDetails MapDefendant(ApiDomain.CaseDefendant defendant)
        {
            return new BusinessDomain.DefendantDetails
            {
                Id = defendant.Id,
                ListOrder = defendant.ListOrder,
                FirstNames = defendant.FirstNames,
                Surname = defendant.Surname,
                // todo: no organisation name in TDE?
                OrganisationName = defendant.Surname,
                Dob = defendant.Dob,
                isYouth = defendant.Youth
            };
        }

        private ApiDomain.CaseDefendant GetLeadDefendantDetails(ApiDomain.CaseDetails caseDetails)
        {
            var summary = caseDetails.Summary;
            var defendants = caseDetails.Defendants;
            // todo: this is not ideal, TDE only gives us the names of the lead defendant, so not 100%
            // that we find the defendant recrod we want (e.g. if there are two John Smiths on the case?) 
            return defendants.FirstOrDefault(defendant => summary.LeadDefendantFirstNames == defendant.FirstNames
                && summary.LeadDefendantSurname == defendant.Surname
                && summary.LeadDefendantType == defendant.Type);
        }
    }
}
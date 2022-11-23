using RumpoleGateway.Domain.CoreDataApi.CaseDetails;

namespace RumpoleGateway.Domain.CaseDataService
{
    public class DocumentArg : CaseArg
    {
        // todo: is this ok to be tied to the business domain CmsDocCategory
        public CmsDocCategory CmsDocCategory { get; set; }
        public int DocumentId { get; set; }
    }
}
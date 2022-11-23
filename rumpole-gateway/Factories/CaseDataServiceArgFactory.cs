

using RumpoleGateway.Domain.CaseDataService;

namespace RumpoleGateway.Factories
{
    public class CaseDataServiceArgFactory : ICaseDataServiceArgFactory
    {
        public CaseArg CreateCaseArgFromUrnArg(UrnArg arg, int caseId)
        {
            return new CaseArg
            {
                OnBehalfOfToken = arg.OnBehalfOfToken,
                UpstreamToken = arg.UpstreamToken,
                CorrelationId = arg.CorrelationId,
                Urn = arg.Urn,
                CaseId = caseId
            };
        }
    }
}
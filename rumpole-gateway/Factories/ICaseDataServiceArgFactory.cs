using RumpoleGateway.Domain.CaseDataService;

namespace RumpoleGateway.Factories
{
    public interface ICaseDataServiceArgFactory
    {
        CaseArg CreateCaseArgFromUrnArg(UrnArg arg, int caseId);
    }
}


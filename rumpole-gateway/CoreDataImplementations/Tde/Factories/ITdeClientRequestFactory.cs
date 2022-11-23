using System.Net.Http;
using RumpoleGateway.Domain.CaseDataService;

namespace RumpoleGateway.CoreDataImplementations.Tde.Factories
{
    public interface ITdeClientRequestFactory
    {
        HttpRequestMessage CreateListCasesRequest(UrnArg arg);

        HttpRequestMessage CreateGetCaseRequest(CaseArg arg);

        HttpRequestMessage CreateListCaseDocumentsRequest(CaseArg arg);
    }
}
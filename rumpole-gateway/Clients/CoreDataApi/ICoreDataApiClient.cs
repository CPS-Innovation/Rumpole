using RumpoleGateway.Domain.CoreDataApi.CaseDetails;
using System.Threading.Tasks;

namespace RumpoleGateway.Clients.CoreDataApi
{
    public interface ICoreDataApiClient
    {
        Task<CaseDetails> GetCaseDetailsById(string caseId, string accessToken);

        //Task<CaseDetails> GetCaseInformatoinByURN(string urn, string accessToken);

    }
}

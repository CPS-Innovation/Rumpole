using RumpoleGateway.Domain.CoreDataApi;
using System.Threading.Tasks;

namespace RumpoleGateway.Clients.CoreDataApi
{
    public interface ICoreDataApiClient
    {
        Task<CaseInformation> GetCaseInformationByUrn();
    }
}

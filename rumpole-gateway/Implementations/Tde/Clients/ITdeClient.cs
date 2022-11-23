using System.Collections.Generic;
using System.Threading.Tasks;
using RumpoleGateway.Domain.CaseDataService;
using RumpoleGateway.Implementations.Tde.Domain;

namespace RumpoleGateway.Implementations.Tde.Clients
{
    public interface ITdeClient
    {
        Task<IEnumerable<CaseIdentifiers>> ListCaseIdsAsync(UrnArg arg);

        Task<Case> GetCaseAsync(CaseArg arg);

        Task<IEnumerable<DocumentDetails>> ListCaseDocumentsAsync(CaseArg arg);
    }
}
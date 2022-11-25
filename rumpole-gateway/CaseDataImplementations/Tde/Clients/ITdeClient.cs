using System.Collections.Generic;
using System.Threading.Tasks;
using RumpoleGateway.Domain.CaseData.Args;
using RumpoleGateway.CaseDataImplementations.Tde.Domain;

namespace RumpoleGateway.CaseDataImplementations.Tde.Clients
{
    public interface ITdeClient
    {
        Task<IEnumerable<CaseIdentifiers>> ListCaseIdsAsync(UrnArg arg);

        Task<CaseDetails> GetCaseAsync(CaseArg arg);

        Task<IEnumerable<DocumentDetails>> ListCaseDocumentsAsync(CaseArg arg);
    }
}
using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Clients.DocumentRedaction
{
    public class DocumentRedactionClientStub : IDocumentRedactionClient
    {
        public Task<DocumentCheckOutStatus> CheckOutDocument(string caseId, string documentId, string accessToken)
        {
            return Task.FromResult(DocumentCheckOutStatus.CheckedOut);
        }
    }
}

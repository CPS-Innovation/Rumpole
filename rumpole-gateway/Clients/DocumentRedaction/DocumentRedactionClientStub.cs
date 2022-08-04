using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Clients.DocumentRedaction
{
    public class DocumentRedactionClientStub : IDocumentRedactionClient
    {
        public Task<DocumentRedactionStatus> CheckOutDocument(string caseId, string documentId, string accessToken)
        {
            return Task.FromResult(DocumentRedactionStatus.CheckedOut);
        }

        public Task<DocumentRedactionStatus> CheckInDocument(string caseId, string documentId, string accessToken)
        {
            return Task.FromResult(DocumentRedactionStatus.CheckedIn);
        }
    }
}

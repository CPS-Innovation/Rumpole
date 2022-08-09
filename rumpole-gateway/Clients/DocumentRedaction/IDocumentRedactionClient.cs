using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Clients.DocumentRedaction
{
    public interface IDocumentRedactionClient
    {
        public Task<DocumentRedactionStatus> CheckOutDocument(string caseId, string documentId, string accessToken);

        public Task<DocumentRedactionStatus> CheckInDocument(string caseId, string documentId, string accessToken);

        public Task<DocumentRedactionSaveResult> SaveRedactions(string caseId, string documentId, string fileName, DocumentRedactionSaveRequest saveRequest, string accessToken);
    }
}

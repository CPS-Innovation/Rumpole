using System;
using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Clients.DocumentRedaction
{
    public interface IDocumentRedactionClient
    {
        public Task<DocumentRedactionStatus> CheckOutDocumentAsync(string caseId, string documentId, string accessToken, Guid correlationId);

        public Task<DocumentRedactionStatus> CheckInDocumentAsync(string caseId, string documentId, string accessToken, Guid correlationId);

        public Task<DocumentRedactionSaveResult> SaveRedactionsAsync(string caseId, string documentId, string fileName, DocumentRedactionSaveRequest saveRequest, string accessToken, Guid correlationId);
    }
}

using System;
using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Clients.DocumentRedaction
{
    public class DocumentRedactionClient : IDocumentRedactionClient
    {
        public Task<DocumentRedactionStatus> CheckInDocumentAsync(string caseId, string documentId, string accessToken)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task<DocumentRedactionSaveResult> SaveRedactionsAsync(string caseId, string documentId, string fileName, DocumentRedactionSaveRequest saveRequest, string accessToken)
        {
            // TODO
            throw new NotImplementedException();
        }

        Task<DocumentRedactionStatus> IDocumentRedactionClient.CheckOutDocumentAsync(string caseId, string documentId, string accessToken)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}

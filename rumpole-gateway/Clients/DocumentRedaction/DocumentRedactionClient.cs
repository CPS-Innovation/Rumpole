using System;
using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Clients.DocumentRedaction
{
    public class DocumentRedactionClient : IDocumentRedactionClient
    {
        public Task<DocumentRedactionStatus> CheckInDocument(string caseId, string documentId, string accessToken)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task<DocumentRedactionSaveResult> SaveRedactions(string caseId, string documentId, string fileName, DocumentRedactionSaveRequest saveRequest, string accessToken)
        {
            // TODO
            throw new NotImplementedException();
        }

        Task<DocumentRedactionStatus> IDocumentRedactionClient.CheckOutDocument(string caseId, string documentId, string accessToken)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}

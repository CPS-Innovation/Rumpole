using System;
using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Clients.DocumentRedaction
{
    public class DocumentRedactionClient : IDocumentRedactionClient
    {
        public Task<DocumentCheckOutStatus> CheckOutDocument(string caseId, string documentId, string accessToken)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}

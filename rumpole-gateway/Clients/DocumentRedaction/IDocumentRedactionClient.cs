using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Clients.DocumentRedaction
{
    public interface IDocumentRedactionClient
    {
        public Task<DocumentCheckOutStatus> CheckOutDocument(string caseId, string documentId, string accessToken);
    }
}

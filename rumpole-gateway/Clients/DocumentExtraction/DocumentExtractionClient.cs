using System;
using System.IO;
using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentExtraction;

namespace RumpoleGateway.Clients.DocumentExtraction
{
	public class DocumentExtractionClient : IDocumentExtractionClient
	{
		public DocumentExtractionClient()
		{
		}

        public Task<Case> GetCaseDocumentsAsync(string caseId, string accessToken)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task<Stream> GetDocumentAsync(string documentId, string fileName, string accessToken)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}


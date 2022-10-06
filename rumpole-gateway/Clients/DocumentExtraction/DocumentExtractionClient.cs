using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentExtraction;

namespace RumpoleGateway.Clients.DocumentExtraction
{
    [ExcludeFromCodeCoverage]
    public class DocumentExtractionClient : IDocumentExtractionClient
	{
		public DocumentExtractionClient()
		{
		}

        public Task<Case> GetCaseDocumentsAsync(string caseId, string accessToken, Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetDocumentAsync(string documentId, string fileName, string accessToken, Guid correlationId)
        {
            throw new NotImplementedException();
        }
    }
}


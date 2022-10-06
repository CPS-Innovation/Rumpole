using System;
using System.IO;
using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentExtraction;

namespace RumpoleGateway.Clients.DocumentExtraction
{
	public interface IDocumentExtractionClient
	{
		Task<Case> GetCaseDocumentsAsync(string caseId, string accessToken, Guid correlationId);

		Task<Stream> GetDocumentAsync(string documentId, string fileName, string accessToken, Guid correlationId);
	}
}


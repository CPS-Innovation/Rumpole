using System.Threading.Tasks;
using RumpoleGateway.Domain.DocumentExtraction;

namespace RumpoleGateway.Clients.DocumentExtraction
{
	public class DocumentExtractionClientStub : IDocumentExtractionClient
	{
		public async Task<Case> GetCaseDocumentsAsync(string caseId, string accessToken)
        {
            return new Case
            {
                CaseId = caseId,
                CaseDocuments = new[]
                {
                    new CaseDocument
                    {
                        DocumentId = "12345",
                        FileName = "test.doc",
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    }
                }
            };
        }
	}
}


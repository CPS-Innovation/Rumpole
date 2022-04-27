using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.DocumentExtraction;
using System;
using System.Threading.Tasks;

namespace RumpoleGateway.Functions.CoreDataApi
{
    public class DocumentExtractionGetDocument
    {
        private readonly IDocumentExtractionClient _documentExtractionClient;
        private readonly ILogger<DocumentExtractionGetCaseDocuments> _logger;

        public DocumentExtractionGetDocument(
            IDocumentExtractionClient documentExtractionClient,
            ILogger<DocumentExtractionGetCaseDocuments> logger)
        {
            _documentExtractionClient = documentExtractionClient;
            _logger = logger;
        }

        [FunctionName("DocumentExtractionGetDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents/{documentId}/{*fileName}")] HttpRequest req, string documentId, string fileName)
        {
            try
            {
                string errorMsg;
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                {
                    errorMsg = "Authorization token is not supplied.";
                    return ErrorResponse(new UnauthorizedObjectResult(errorMsg), errorMsg);
                }

                if (string.IsNullOrWhiteSpace(documentId))
                {
                    errorMsg = "Document id is not supplied.";
                    return ErrorResponse(new BadRequestObjectResult(errorMsg), errorMsg);
                }

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    errorMsg = "FileName is not supplied.";
                    return ErrorResponse(new BadRequestObjectResult(errorMsg), errorMsg);
                }

                //TODO exchange access token via on behalf of?
                var document = await _documentExtractionClient.GetDocumentAsync(documentId, fileName, "accessToken");

                if (document != null)
                {
                    return new OkObjectResult(document);
                }

                errorMsg = $"No document found for file name '{fileName}'."; // TODO change this to document id when hooked up to CDE
                return ErrorResponse(new NotFoundObjectResult(errorMsg), errorMsg);
            }
            catch(Exception exception)
            {
                return exception switch
                {
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.")
                };
            }
        }

        private IActionResult ErrorResponse(IActionResult result, string message)
        {
            _logger.LogError(message);
            return result;
        }

        private IActionResult InternalServerErrorResponse(Exception exception, string baseErrorMessage)
        {
            _logger.LogError(exception, baseErrorMessage);
            return new StatusCodeResult(500);
        }
    }
}


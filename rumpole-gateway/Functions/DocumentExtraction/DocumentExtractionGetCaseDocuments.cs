using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.DocumentExtraction;

namespace RumpoleGateway.Functions.DocumentExtraction
{
    public class DocumentExtractionGetCaseDocuments
    {
        private readonly IDocumentExtractionClient _documentExtractionClient;
        private readonly ILogger<DocumentExtractionGetCaseDocuments> _logger;

        public DocumentExtractionGetCaseDocuments(
            IDocumentExtractionClient documentExtractionClient,
            ILogger<DocumentExtractionGetCaseDocuments> logger)
        {
            _documentExtractionClient = documentExtractionClient;
            _logger = logger;
        }

        [FunctionName("DocumentExtractionGetCaseDocuments")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "case-documents/{caseId}")] HttpRequest req, string caseId)
        {
            try
            {
                string errorMsg;
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                {
                    errorMsg = "Authorization token is not supplied.";
                    return ErrorResponse(new UnauthorizedObjectResult(errorMsg), errorMsg);
                }

                if (!int.TryParse(caseId, out var _))
                {
                    errorMsg = "Invalid case id. A 32-bit integer is required.";
                    return ErrorResponse(new BadRequestObjectResult(errorMsg), errorMsg);
                }

                //TODO exchange access token via on behalf of?
                var caseDocuments = await _documentExtractionClient.GetCaseDocumentsAsync(caseId, "accessToken");

                if (caseDocuments != null)
                {
                    return new OkObjectResult(caseDocuments);
                }

                errorMsg = $"No data found for case id '{caseId}'.";
                return ErrorResponse(new NotFoundObjectResult(errorMsg), errorMsg);
            }
            catch (Exception exception)
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


using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.DocumentExtraction;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions.DocumentExtraction
{
    public class DocumentExtractionGetDocument : BaseRumpoleFunction
    {
        private readonly IDocumentExtractionClient _documentExtractionClient;
        private readonly IAuthorizationValidator _tokenValidator;
        private readonly ILogger<DocumentExtractionGetDocument> _logger;

        public DocumentExtractionGetDocument(IDocumentExtractionClient documentExtractionClient, ILogger<DocumentExtractionGetDocument> logger, IAuthorizationValidator tokenValidator)
            : base(logger)
        {
            _documentExtractionClient = documentExtractionClient;
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
            _logger = logger;
        }

        [FunctionName("DocumentExtractionGetDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents/{documentId}/{*fileName}")] HttpRequest req, string documentId, string fileName)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "DocumentExtractionGetDocument - Run";

            try
            {
                if (!req.Headers.TryGetValue("Correlation-Id", out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
                    return BadRequestErrorResponse("Invalid correlationId. A valid GUID is required.", currentCorrelationId, loggingName);

                if (!Guid.TryParse(correlationId, out currentCorrelationId))
                    if (currentCorrelationId == Guid.Empty)
                        return BadRequestErrorResponse("Invalid correlationId. A valid GUID is required.", currentCorrelationId, loggingName);

                _logger.LogMethodEntry(currentCorrelationId, loggingName, string.Empty);

                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse(currentCorrelationId, loggingName);

                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken, currentCorrelationId);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed", currentCorrelationId, loggingName);

                if (string.IsNullOrWhiteSpace(documentId))
                    return BadRequestErrorResponse("Document id is not supplied.", currentCorrelationId, loggingName);

                if (string.IsNullOrWhiteSpace(fileName))
                    return BadRequestErrorResponse("FileName is not supplied.", currentCorrelationId, loggingName);

                // exchange access token via on behalf of?
                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Getting document stream for '{documentId}'");
                var document = await _documentExtractionClient.GetDocumentAsync(documentId, fileName, "accessToken", currentCorrelationId);

                return document != null
                    ? new OkObjectResult(document)
                    : NotFoundErrorResponse($"No document found for file name '{fileName}'.", currentCorrelationId, loggingName);
                // change this to document id when hooked up to CDE?
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.", currentCorrelationId, loggingName)
                };
            }
            finally
            {
                _logger.LogMethodExit(currentCorrelationId, loggingName, string.Empty);
            }
        }
    }
}


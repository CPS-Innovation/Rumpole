using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.DocumentExtraction;
using RumpoleGateway.Domain.DocumentExtraction;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Functions.DocumentExtraction
{
    public class DocumentExtractionGetCaseDocuments : BaseRumpoleFunction
    {
        private readonly IDocumentExtractionClient _documentExtractionClient;
        private readonly IAuthorizationValidator _tokenValidator;
        private readonly ILogger<DocumentExtractionGetCaseDocuments> _logger;

        public DocumentExtractionGetCaseDocuments(IDocumentExtractionClient documentExtractionClient, ILogger<DocumentExtractionGetCaseDocuments> logger, 
            IAuthorizationValidator tokenValidator)
            : base(logger)
        {
            _documentExtractionClient = documentExtractionClient;
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
            _logger = logger;
        }

        [FunctionName("DocumentExtractionGetCaseDocuments")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "case-documents/{caseId}")] HttpRequest req, string caseId)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "DocumentExtractionGetCaseDocuments - Run";
            Case caseDocuments = null;

            try
            {
                if (!req.Headers.TryGetValue("Correlation-Id", out var correlationId) ||
                    string.IsNullOrWhiteSpace(correlationId))
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

                if (!int.TryParse(caseId, out var _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.", currentCorrelationId, loggingName);

                //exchange access token via on behalf of?
                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Getting case documents {caseId}");
                caseDocuments = await _documentExtractionClient.GetCaseDocumentsAsync(caseId, "accessToken", currentCorrelationId);

                return caseDocuments != null
                    ? new OkObjectResult(caseDocuments)
                    : NotFoundErrorResponse($"No data found for case id '{caseId}'.", currentCorrelationId, loggingName);
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
                _logger.LogMethodExit(currentCorrelationId, loggingName, caseDocuments.ToJson());
            }
        }
    }
}


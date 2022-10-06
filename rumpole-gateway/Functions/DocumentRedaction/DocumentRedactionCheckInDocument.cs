using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.DocumentRedaction;
using RumpoleGateway.Domain.DocumentRedaction;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Functions.DocumentRedaction
{
    public class DocumentRedactionCheckInDocument : BaseRumpoleFunction
    {
        private readonly IDocumentRedactionClient _documentRedactionClient;
        private readonly IAuthorizationValidator _tokenValidator;
        private readonly ILogger<DocumentRedactionCheckInDocument> _logger;

        public DocumentRedactionCheckInDocument(ILogger<DocumentRedactionCheckInDocument> logger, IDocumentRedactionClient documentRedactionClient, IAuthorizationValidator tokenValidator)
            : base(logger)
        {
            _documentRedactionClient = documentRedactionClient ?? throw new ArgumentNullException(nameof(documentRedactionClient));
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
            _logger = logger;
        }

        [FunctionName("DocumentRedactionCheckInDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "documents/checkin/{caseId}/{documentId}")] HttpRequest req, string caseId, string documentId)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "DocumentRedactionCheckInDocument - Run";
            var checkinResult = DocumentRedactionStatus.NotFound;

            try
            {
                if (!req.Headers.TryGetValue("X-Correlation-ID", out var correlationId) ||
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

                if (string.IsNullOrWhiteSpace(documentId))
                    return BadRequestErrorResponse("Document id is not supplied.", currentCorrelationId, loggingName);

                if (!int.TryParse(caseId, out _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.", currentCorrelationId, loggingName);

                //exchange access token via on behalf of?
                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Check-in document for caseId: {caseId}, documentId: {documentId}");
                checkinResult = await _documentRedactionClient.CheckInDocumentAsync(caseId, documentId, accessToken, currentCorrelationId);
                return checkinResult switch
                {
                    DocumentRedactionStatus.CheckedIn => new OkObjectResult(new DocumentStatusChangeResult(true, checkinResult)),
                    DocumentRedactionStatus.NotFound => NotFoundErrorResponse($"No document found for caseId '{caseId}' and documentId '{documentId}'.", currentCorrelationId,
                        loggingName),
                    _ => throw new ArgumentOutOfRangeException($"Invalid document checkout status returned: {checkinResult}")
                };
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
                _logger.LogMethodExit(currentCorrelationId, loggingName, checkinResult.ToJson());
            }
        }
    }
}

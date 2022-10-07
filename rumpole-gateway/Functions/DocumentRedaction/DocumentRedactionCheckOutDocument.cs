using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.DocumentRedaction;
using RumpoleGateway.Domain.DocumentRedaction;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Functions.DocumentRedaction
{
    public class DocumentRedactionCheckOutDocument : BaseRumpoleFunction
    {
        private readonly IDocumentRedactionClient _documentRedactionClient;
        private readonly IAuthorizationValidator _tokenValidator;
        private readonly ILogger<DocumentRedactionCheckOutDocument> _logger;

        public DocumentRedactionCheckOutDocument(ILogger<DocumentRedactionCheckOutDocument> logger, IDocumentRedactionClient documentRedactionClient, IAuthorizationValidator tokenValidator)
            : base(logger)
        {
            _documentRedactionClient = documentRedactionClient ?? throw new ArgumentNullException(nameof(documentRedactionClient));
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
            _logger = logger;
        }

        [FunctionName("DocumentRedactionCheckOutDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "documents/checkout/{caseId}/{documentId}")] HttpRequest req, string caseId, string documentId)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "DocumentRedactionCheckOutDocument - Run";
            var checkoutResult = DocumentRedactionStatus.NotFound;

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

                if (string.IsNullOrWhiteSpace(documentId))
                    return BadRequestErrorResponse("Document id is not supplied.", currentCorrelationId, loggingName);

                if (!int.TryParse(caseId, out _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.", currentCorrelationId, loggingName);

                //exchange access token via on behalf of?
                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Checking out document for caseId: {caseId}, documentId: {documentId}");
                checkoutResult = await _documentRedactionClient.CheckOutDocumentAsync(caseId, documentId, accessToken, currentCorrelationId);
                return checkoutResult switch
                {
                    DocumentRedactionStatus.CheckedOut => new OkObjectResult(new DocumentStatusChangeResult(true, checkoutResult)),
                    DocumentRedactionStatus.AlreadyCheckedOut => new OkObjectResult(new DocumentStatusChangeResult(false, checkoutResult)),
                    DocumentRedactionStatus.NotFound => NotFoundErrorResponse($"No document found for caseId '{caseId}' and documentId '{documentId}'.", currentCorrelationId,
                        loggingName),
                    _ => throw new ArgumentOutOfRangeException($"Invalid document checkout status returned: {checkoutResult}")
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
                _logger.LogMethodExit(currentCorrelationId, loggingName, checkoutResult.ToJson());
            }
        }
    }
}

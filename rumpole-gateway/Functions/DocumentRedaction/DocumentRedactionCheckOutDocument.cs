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
using RumpoleGateway.Helpers.Extension;

namespace RumpoleGateway.Functions.DocumentRedaction
{
    public class DocumentRedactionCheckOutDocument : BaseRumpoleFunction
    {
        private readonly IDocumentRedactionClient _documentRedactionClient;
        private readonly ILogger<DocumentRedactionCheckOutDocument> _logger;

        public DocumentRedactionCheckOutDocument(ILogger<DocumentRedactionCheckOutDocument> logger, IDocumentRedactionClient documentRedactionClient, IAuthorizationValidator tokenValidator)
            : base(logger, tokenValidator)
        {
            _documentRedactionClient = documentRedactionClient ?? throw new ArgumentNullException(nameof(documentRedactionClient));
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
                var validationResult = await ValidateRequest(req, loggingName);
                if (validationResult.InvalidResponseResult != null)
                    return validationResult.InvalidResponseResult;
                
                _logger.LogMethodEntry(currentCorrelationId, loggingName, string.Empty);

                if (string.IsNullOrWhiteSpace(documentId))
                    return BadRequestErrorResponse("Document id is not supplied.", currentCorrelationId, loggingName);

                if (!int.TryParse(caseId, out _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.", currentCorrelationId, loggingName);

                //exchange access token via on behalf of?
                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Checking out document for caseId: {caseId}, documentId: {documentId}");
                checkoutResult = await _documentRedactionClient.CheckOutDocumentAsync(caseId, documentId, validationResult.AccessTokenValue.ToJwtString(), currentCorrelationId);
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

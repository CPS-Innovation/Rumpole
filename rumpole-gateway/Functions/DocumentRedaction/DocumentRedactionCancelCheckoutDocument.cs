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
using RumpoleGateway.Helpers.Extension;

namespace RumpoleGateway.Functions.DocumentRedaction
{
    public class DocumentRedactionCancelCheckoutDocument : BaseRumpoleFunction
    {
        private readonly IDocumentRedactionClient _documentRedactionClient;
        private readonly ILogger<DocumentRedactionCancelCheckoutDocument> _logger;

        public DocumentRedactionCancelCheckoutDocument(ILogger<DocumentRedactionCancelCheckoutDocument> logger, IDocumentRedactionClient documentRedactionClient, IAuthorizationValidator tokenValidator)
            : base(logger, tokenValidator)
        {
            _documentRedactionClient = documentRedactionClient ?? throw new ArgumentNullException(nameof(documentRedactionClient));
            _logger = logger;
        }

        [FunctionName("DocumentRedactionCancelCheckoutDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "urns/${urn}/cases/{caseId}/documents/{documentId}/checkout")] HttpRequest req, string caseId, string documentId)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "DocumentRedactionCheckInDocument - Run";
            var checkinResult = DocumentRedactionStatus.NotFound;

            try
            {
                var validationResult = await ValidateRequest(req, loggingName, ValidRoles.UserImpersonation);
                if (validationResult.InvalidResponseResult != null)
                    return validationResult.InvalidResponseResult;

                currentCorrelationId = validationResult.CurrentCorrelationId;
                _logger.LogMethodEntry(currentCorrelationId, loggingName, string.Empty);

                if (string.IsNullOrWhiteSpace(documentId))
                    return BadRequestErrorResponse("Document id is not supplied.", currentCorrelationId, loggingName);

                if (!int.TryParse(caseId, out _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.", currentCorrelationId, loggingName);

                //exchange access token via on behalf of?
                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Check-in document for caseId: {caseId}, documentId: {documentId}");
                checkinResult = await _documentRedactionClient.CheckInDocumentAsync(caseId, documentId, validationResult.AccessTokenValue.ToJwtString(), currentCorrelationId);
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

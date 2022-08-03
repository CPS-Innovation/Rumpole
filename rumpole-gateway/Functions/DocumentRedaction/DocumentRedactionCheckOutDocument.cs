using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.DocumentRedaction;
using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Functions.DocumentRedaction
{
    public class DocumentRedactionCheckOutDocument
    {
        private readonly ILogger<DocumentRedactionCheckOutDocument> _logger;
        private readonly IDocumentRedactionClient _documentRedactionClient;

        public DocumentRedactionCheckOutDocument(ILogger<DocumentRedactionCheckOutDocument> logger, IDocumentRedactionClient documentRedactionClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _documentRedactionClient = documentRedactionClient ?? throw new ArgumentNullException(nameof(documentRedactionClient));
        }

        [FunctionName("DocumentRedactionCheckOutDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "documents/checkout/{caseId}/{documentId}")] HttpRequest req, string caseId, string documentId)
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

                if (!int.TryParse(caseId, out _))
                {
                    errorMsg = "Invalid case id. A 32-bit integer is required.";
                    return ErrorResponse(new BadRequestObjectResult(errorMsg), errorMsg);
                }

                var checkoutResult = await _documentRedactionClient.CheckOutDocument(caseId, documentId, accessToken);
                switch (checkoutResult)
                {
                    case DocumentCheckOutStatus.CheckedOut:
                        return new OkObjectResult(new DocumentCheckOutResult(true, checkoutResult));
                    case DocumentCheckOutStatus.AlreadyCheckedOut:
                        return new OkObjectResult(new DocumentCheckOutResult(false, checkoutResult));
                    case DocumentCheckOutStatus.NotFound:
                        errorMsg = $"No document found for caseId '{caseId}' and documentId '{documentId}'.";
                        return ErrorResponse(new NotFoundObjectResult(errorMsg), errorMsg);
                    case DocumentCheckOutStatus.SystemError:
                        return new StatusCodeResult(500);
                    default:
                        return new StatusCodeResult(500);
                }
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

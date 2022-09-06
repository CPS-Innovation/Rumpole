using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.DocumentRedaction;
using RumpoleGateway.Domain.DocumentRedaction;
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions.DocumentRedaction
{
    public class DocumentRedactionCheckInDocument : BaseRumpoleFunction
    {
        private readonly IDocumentRedactionClient _documentRedactionClient;
        private readonly IJwtBearerValidator _tokenValidator;

        public DocumentRedactionCheckInDocument(ILogger<DocumentRedactionCheckInDocument> logger, IDocumentRedactionClient documentRedactionClient, IJwtBearerValidator tokenValidator)
            : base(logger)
        {
            _documentRedactionClient = documentRedactionClient ?? throw new ArgumentNullException(nameof(documentRedactionClient));
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
        }

        [FunctionName("DocumentRedactionCheckInDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "documents/checkin/{caseId}/{documentId}")] HttpRequest req, string caseId, string documentId)
        {
            try
            {
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse();

                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed");

                if (string.IsNullOrWhiteSpace(documentId))
                    return BadRequestErrorResponse("Document id is not supplied.");

                if (!int.TryParse(caseId, out _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.");

                var checkoutResult = await _documentRedactionClient.CheckInDocumentAsync(caseId, documentId, accessToken);
                return checkoutResult switch
                {
                    DocumentRedactionStatus.CheckedIn => new OkObjectResult(new DocumentStatusChangeResult(true, checkoutResult)),
                    DocumentRedactionStatus.NotFound => NotFoundErrorResponse($"No document found for caseId '{caseId}' and documentId '{documentId}'."),
                    _ => throw new ArgumentOutOfRangeException($"Invalid document checkout status returned: {checkoutResult}")
                };
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.")
                };
            }
        }
    }
}

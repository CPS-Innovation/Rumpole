using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.DocumentRedaction;
using RumpoleGateway.Domain.DocumentRedaction;
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions.DocumentRedaction
{
    public class DocumentRedactionCheckOutDocument : BaseRumpoleFunction
    {
        private readonly IDocumentRedactionClient _documentRedactionClient;
        private readonly ITokenValidator _tokenValidator;

        public DocumentRedactionCheckOutDocument(ILogger<DocumentRedactionCheckOutDocument> logger, IDocumentRedactionClient documentRedactionClient, ITokenValidator tokenValidator)
            : base(logger)
        {
            _documentRedactionClient = documentRedactionClient ?? throw new ArgumentNullException(nameof(documentRedactionClient));
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
        }

        [FunctionName("DocumentRedactionCheckOutDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "documents/checkout/{caseId}/{documentId}")] HttpRequest req, string caseId, string documentId)
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
                
                var checkoutResult = await _documentRedactionClient.CheckOutDocumentAsync(caseId, documentId, accessToken);
                return checkoutResult switch
                {
                    DocumentRedactionStatus.CheckedOut => new OkObjectResult(new DocumentStatusChangeResult(true, checkoutResult)),
                    DocumentRedactionStatus.AlreadyCheckedOut => new OkObjectResult(new DocumentStatusChangeResult(false, checkoutResult)),
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

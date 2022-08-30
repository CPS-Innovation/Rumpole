using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.DocumentExtraction;
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions.DocumentExtraction
{
    public class DocumentExtractionGetCaseDocuments : BaseRumpoleFunction
    {
        private readonly IDocumentExtractionClient _documentExtractionClient;
        private readonly ITokenValidator _tokenValidator;

        public DocumentExtractionGetCaseDocuments(IDocumentExtractionClient documentExtractionClient, ILogger<DocumentExtractionGetCaseDocuments> logger, ITokenValidator tokenValidator)
            : base(logger)
        {
            _documentExtractionClient = documentExtractionClient;
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
        }

        [FunctionName("DocumentExtractionGetCaseDocuments")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "case-documents/{caseId}")] HttpRequest req, string caseId)
        {
            try
            {
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse();

                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed");

                if (!int.TryParse(caseId, out var _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.");
                
                //TODO exchange access token via on behalf of?
                var caseDocuments = await _documentExtractionClient.GetCaseDocumentsAsync(caseId, "accessToken");

                return caseDocuments != null ? new OkObjectResult(caseDocuments) : NotFoundErrorResponse($"No data found for case id '{caseId}'.");
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


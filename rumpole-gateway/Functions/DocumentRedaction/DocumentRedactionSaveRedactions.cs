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
using RumpoleGateway.Helpers.Extension;

namespace RumpoleGateway.Functions.DocumentRedaction
{
    public class DocumentRedactionSaveRedactions : BaseRumpoleFunction
    {
        private readonly IDocumentRedactionClient _documentRedactionClient;

        public DocumentRedactionSaveRedactions(ILogger<DocumentRedactionSaveRedactions> logger, IDocumentRedactionClient documentRedactionClient)
            : base(logger)
        {
            _documentRedactionClient = documentRedactionClient ?? throw new ArgumentNullException(nameof(documentRedactionClient));
        }

        [FunctionName("DocumentRedactionSaveRedactions")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "documents/saveRedactions/{caseId}/{documentId}/{*fileName}")] HttpRequest req, 
                string caseId, string documentId, string fileName)
        {
            try
            {
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse();

                if (string.IsNullOrWhiteSpace(documentId))
                    return BadRequestErrorResponse("Document id is not supplied.");

                if (!int.TryParse(caseId, out _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.");

                if (string.IsNullOrWhiteSpace(fileName))
                    return BadRequestErrorResponse("Invalid filename - not details received");

                var redactions = await req.GetJsonBody<DocumentRedactionSaveRequest, DocumentRedactionSaveRequestValidator>();
                if (!redactions.IsValid)
                {
                    LogInformation("Invalid redaction request");
                    return redactions.ToBadRequest();
                }
                
                //TODO exchange access token via on behalf of?
                var saveRedactionResult = await _documentRedactionClient.SaveRedactions(caseId, documentId, fileName, redactions.Value, accessToken);
                return saveRedactionResult is {Succeeded: true} ? new OkObjectResult(saveRedactionResult)
                    : string.IsNullOrWhiteSpace(saveRedactionResult.Message) 
                        ? BadRequestErrorResponse($"The redaction request could not be processed for file name '{fileName}'.") : BadRequestErrorResponse(saveRedactionResult.Message);
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

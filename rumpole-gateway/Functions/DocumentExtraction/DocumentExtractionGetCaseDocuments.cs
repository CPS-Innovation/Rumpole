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
        private readonly ILogger<DocumentExtractionGetCaseDocuments> _logger;

        public DocumentExtractionGetCaseDocuments(IDocumentExtractionClient documentExtractionClient, ILogger<DocumentExtractionGetCaseDocuments> logger, 
            IAuthorizationValidator tokenValidator)
            : base(logger, tokenValidator)
        {
            _documentExtractionClient = documentExtractionClient;
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
                var validationResult = await ValidateRequest(req, loggingName, ValidRoles.UserImpersonation);
                if (validationResult.InvalidResponseResult != null)
                    return validationResult.InvalidResponseResult;
                
                currentCorrelationId = validationResult.CurrentCorrelationId;
                _logger.LogMethodEntry(currentCorrelationId, loggingName, string.Empty);

                if (!int.TryParse(caseId, out _))
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


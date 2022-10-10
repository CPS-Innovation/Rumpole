using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using RumpoleGateway.Clients.DocumentRedaction;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Domain.DocumentRedaction;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Helpers.Extension;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Functions.DocumentRedaction
{
    public class DocumentRedactionSaveRedactions : BaseRumpoleFunction
    {
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly IDocumentRedactionClient _documentRedactionClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentRedactionSaveRedactions> _logger;

        public DocumentRedactionSaveRedactions(ILogger<DocumentRedactionSaveRedactions> logger, IOnBehalfOfTokenClient onBehalfOfTokenClient, IDocumentRedactionClient documentRedactionClient,
            IConfiguration configuration, IAuthorizationValidator tokenValidator)
            : base(logger, tokenValidator)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient ?? throw new ArgumentNullException(nameof(onBehalfOfTokenClient));
            _documentRedactionClient = documentRedactionClient ?? throw new ArgumentNullException(nameof(documentRedactionClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName("DocumentRedactionSaveRedactions")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "documents/saveRedactions/{caseId}/{documentId}/{*fileName}")] HttpRequest req, 
                string caseId, string documentId, string fileName)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "DocumentRedactionSaveRedactions - Run";
            DocumentRedactionSaveResult saveRedactionResult = null;

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

                if (string.IsNullOrWhiteSpace(fileName))
                    return BadRequestErrorResponse("Invalid filename - not details received", currentCorrelationId, loggingName);

                var redactions = await req.GetJsonBody<DocumentRedactionSaveRequest, DocumentRedactionSaveRequestValidator>();
                if (!redactions.IsValid)
                {
                    LogInformation("Invalid redaction request", currentCorrelationId, loggingName);
                    return redactions.ToBadRequest();
                }

                var pdfPipelineScope = _configuration["RumpolePipelineRedactPdfScope"];
                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Getting an access token as part of OBO for the following scope {pdfPipelineScope}");
                var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessTokenAsync(validationResult.AccessTokenValue.ToJwtString(), pdfPipelineScope, currentCorrelationId);
                
                //exchange access token via on behalf of for ultimate Cde access?
                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Saving redaction details to the document for {caseId}, documentId {documentId}, fileName {fileName}");
                saveRedactionResult = await _documentRedactionClient.SaveRedactionsAsync(caseId, HttpUtility.UrlDecode(documentId), HttpUtility.UrlDecode(fileName),
                    redactions.Value, onBehalfOfAccessToken, currentCorrelationId);
                return saveRedactionResult is {Succeeded: true}
                    ? new OkObjectResult(saveRedactionResult)
                    : string.IsNullOrWhiteSpace(saveRedactionResult.Message)
                        ? BadRequestErrorResponse($"The redaction request could not be processed for file name '{fileName}'.", currentCorrelationId, loggingName)
                        : BadRequestErrorResponse(saveRedactionResult.Message, currentCorrelationId, loggingName);
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    MsalException => InternalServerErrorResponse(exception, "An onBehalfOfToken exception occurred.", currentCorrelationId, loggingName),
                    HttpRequestException => InternalServerErrorResponse(exception, "A document redaction client http exception occurred.", currentCorrelationId, loggingName),
                    _ => InternalServerErrorResponse(exception, $"An unhandled exception occurred - \"{exception.Message}\"", currentCorrelationId, loggingName)
                };
            }
            finally
            {
                _logger.LogMethodExit(currentCorrelationId, loggingName, saveRedactionResult.ToJson());
            }
        }
    }
}

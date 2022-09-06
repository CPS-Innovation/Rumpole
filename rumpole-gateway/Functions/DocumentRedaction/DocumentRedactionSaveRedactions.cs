using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using RumpoleGateway.Clients.DocumentRedaction;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Domain.DocumentRedaction;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Helpers.Extension;

namespace RumpoleGateway.Functions.DocumentRedaction
{
    public class DocumentRedactionSaveRedactions : BaseRumpoleFunction
    {
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly IDocumentRedactionClient _documentRedactionClient;
        private readonly IConfiguration _configuration;
        private readonly IJwtBearerValidator _tokenValidator;

        public DocumentRedactionSaveRedactions(ILogger<DocumentRedactionSaveRedactions> logger, IOnBehalfOfTokenClient onBehalfOfTokenClient, IDocumentRedactionClient documentRedactionClient,
            IConfiguration configuration, IJwtBearerValidator tokenValidator)
            : base(logger)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient ?? throw new ArgumentNullException(nameof(onBehalfOfTokenClient));
            _documentRedactionClient = documentRedactionClient ?? throw new ArgumentNullException(nameof(documentRedactionClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
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

                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed");

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

                var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessTokenAsync(accessToken.ToJwtString(), _configuration["RumpolePipelineRedactPdfScope"]);

                var saveRedactionResult = await _documentRedactionClient.SaveRedactionsAsync(caseId, HttpUtility.UrlDecode(documentId), HttpUtility.UrlDecode(fileName), 
                    redactions.Value, onBehalfOfAccessToken);
                return saveRedactionResult is {Succeeded: true} ? new OkObjectResult(saveRedactionResult)
                    : string.IsNullOrWhiteSpace(saveRedactionResult.Message) 
                        ? BadRequestErrorResponse($"The redaction request could not be processed for file name '{fileName}'.") : BadRequestErrorResponse(saveRedactionResult.Message);
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    MsalException => InternalServerErrorResponse(exception, "An onBehalfOfToken exception occurred."),
                    HttpRequestException => InternalServerErrorResponse(exception, "A document redaction client http exception occurred."),
                    _ => InternalServerErrorResponse(exception, $"An unhandled exception occurred - \"{exception.Message}\"")
                };
            }
        }
    }
}

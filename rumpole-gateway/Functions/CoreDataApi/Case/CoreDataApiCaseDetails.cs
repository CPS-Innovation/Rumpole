using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using RumpoleGateway.Clients.CoreDataApi;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Domain.CoreDataApi;
using RumpoleGateway.Helpers.Extension;
using System;
using System.Threading.Tasks;
using RumpoleGateway.Domain.CoreDataApi.CaseDetails;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Functions.CoreDataApi.Case
{
    public class CoreDataApiCaseDetails : BaseRumpoleFunction
    {
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly ICoreDataApiClient _coreDataApiClient;
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationValidator _tokenValidator;
        private readonly ILogger<CoreDataApiCaseDetails> _logger;

        public CoreDataApiCaseDetails(ILogger<CoreDataApiCaseDetails> logger, IOnBehalfOfTokenClient onBehalfOfTokenClient, ICoreDataApiClient coreDataApiClient,
                                 IConfiguration configuration, IAuthorizationValidator tokenValidator)
        : base(logger)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _coreDataApiClient = coreDataApiClient;
            _configuration = configuration;
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
            _logger = logger;
        }

        [FunctionName("CoreDataApiCaseDetails")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "case-details/{caseId}")] HttpRequest req, string caseId)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "CoreDataApiCaseDetails - Run";
            CaseDetails caseDetails = null;

            try
            {
                if (!req.Headers.TryGetValue("Correlation-Id", out var correlationId) ||
                    string.IsNullOrWhiteSpace(correlationId))
                    return BadRequestErrorResponse("Invalid correlationId. A valid GUID is required.", currentCorrelationId, loggingName);

                if (!Guid.TryParse(correlationId, out currentCorrelationId))
                    if (currentCorrelationId == Guid.Empty)
                        return BadRequestErrorResponse("Invalid correlationId. A valid GUID is required.", currentCorrelationId, loggingName);

                _logger.LogMethodEntry(currentCorrelationId, loggingName, string.Empty);

                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) ||
                    string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse(currentCorrelationId, loggingName);

                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken, currentCorrelationId);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed", currentCorrelationId, loggingName);

                var cdaScope = _configuration["CoreDataApiScope"]; 
                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Getting an access token as part of OBO for the following scope {cdaScope}");
                var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessTokenAsync(accessToken.ToJwtString(), cdaScope, currentCorrelationId);

                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Getting case details by Id {caseId}");
                caseDetails = await _coreDataApiClient.GetCaseDetailsByIdAsync(caseId, onBehalfOfAccessToken, currentCorrelationId);

                return caseDetails != null
                    ? new OkObjectResult(caseDetails)
                    : NotFoundErrorResponse($"No data found for case id '{caseId}'.", currentCorrelationId, loggingName);
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    MsalException => InternalServerErrorResponse(exception, "An MSAL exception occurred.", currentCorrelationId, loggingName),
                    CoreDataApiException => InternalServerErrorResponse(exception, "A core data api exception occurred.", currentCorrelationId, loggingName),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.", currentCorrelationId, loggingName)
                };
            }
            finally
            {
                _logger.LogMethodExit(currentCorrelationId, loggingName, caseDetails.ToJson());
            }
        }
    }
}


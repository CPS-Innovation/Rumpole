using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Helpers.Extension;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineGetTracker : BaseRumpoleFunction
    {
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly IPipelineClient _pipelineClient;
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationValidator _tokenValidator;
        private readonly ILogger<RumpolePipelineGetTracker> _logger;

        public RumpolePipelineGetTracker(ILogger<RumpolePipelineGetTracker> logger, IOnBehalfOfTokenClient onBehalfOfTokenClient, IPipelineClient pipelineClient,
                                 IConfiguration configuration, IAuthorizationValidator tokenValidator)
        : base(logger)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _pipelineClient = pipelineClient;
            _configuration = configuration;
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
            _logger = logger;
        }

        [FunctionName("RumpolePipelineGetTracker")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cases/{caseId}/tracker")] HttpRequest req, string caseId)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "RumpolePipelineGetTracker - Run";
            Tracker tracker = null;

            try
            {
                if (!req.Headers.TryGetValue("X-Correlation-ID", out var correlationId) ||
                    string.IsNullOrWhiteSpace(correlationId))
                    return BadRequestErrorResponse("Invalid correlationId. A valid GUID is required.", currentCorrelationId, loggingName);

                if (!Guid.TryParse(correlationId, out currentCorrelationId))
                    if (currentCorrelationId == Guid.Empty)
                        return BadRequestErrorResponse("Invalid correlationId. A valid GUID is required.", currentCorrelationId, loggingName);

                _logger.LogMethodEntry(currentCorrelationId, loggingName, string.Empty);

                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse(currentCorrelationId, loggingName);

                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken, currentCorrelationId);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed", currentCorrelationId, loggingName);

                if (!int.TryParse(caseId, out var _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.", currentCorrelationId, loggingName);

                var coordinatorScope = _configuration["RumpolePipelineCoordinatorScope"];
                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Getting an access token as part of OBO for the following scope {coordinatorScope}");
                var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessTokenAsync(accessToken.ToJwtString(), coordinatorScope, currentCorrelationId);

                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Getting tracker details for caseId {caseId}");
                tracker = await _pipelineClient.GetTrackerAsync(caseId, onBehalfOfAccessToken, currentCorrelationId);

                return tracker == null ? NotFoundErrorResponse($"No tracker found for case id '{caseId}'.", currentCorrelationId, loggingName) : new OkObjectResult(tracker);
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    MsalException => InternalServerErrorResponse(exception, "An onBehalfOfToken exception occurred.", currentCorrelationId, loggingName),
                    HttpRequestException => InternalServerErrorResponse(exception, "A pipeline client http exception occurred when calling GetTracker.", currentCorrelationId,
                        loggingName),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.", currentCorrelationId, loggingName)
                };
            }
            finally
            {
                _logger.LogMethodExit(currentCorrelationId, loggingName, tracker.ToJson());
            }
        }
    }
}


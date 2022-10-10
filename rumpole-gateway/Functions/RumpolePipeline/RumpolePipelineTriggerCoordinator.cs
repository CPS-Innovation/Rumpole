using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Factories;
using RumpoleGateway.Helpers.Extension;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineTriggerCoordinator : BaseRumpoleFunction
    {
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly IPipelineClient _pipelineClient;
        private readonly IConfiguration _configuration;
        private readonly ITriggerCoordinatorResponseFactory _triggerCoordinatorResponseFactory;
        private readonly ILogger<RumpolePipelineTriggerCoordinator> _logger;

        public RumpolePipelineTriggerCoordinator(ILogger<RumpolePipelineTriggerCoordinator> logger, IOnBehalfOfTokenClient onBehalfOfTokenClient,
                                 IPipelineClient pipelineClient, IConfiguration configuration,
                                 ITriggerCoordinatorResponseFactory triggerCoordinatorResponseFactory, IAuthorizationValidator tokenValidator)
        : base(logger, tokenValidator)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _pipelineClient = pipelineClient;
            _configuration = configuration;
            _triggerCoordinatorResponseFactory = triggerCoordinatorResponseFactory;
            _logger = logger;
        }

        [FunctionName("RumpolePipelineTriggerCoordinator")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cases/{caseId}")] HttpRequest req, string caseId)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "RumpolePipelineTriggerCoordinator - Run";

            try
            {
                var validationResult = await ValidateRequest(req, loggingName);
                if (validationResult.InvalidResponseResult != null)
                    return validationResult.InvalidResponseResult;
                
                _logger.LogMethodEntry(currentCorrelationId, loggingName, string.Empty);

                if (!int.TryParse(caseId, out var _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.", currentCorrelationId, loggingName);

                var force = false;
                if (req.Query.ContainsKey("force") && !bool.TryParse(req.Query["force"], out force))
                    return BadRequestErrorResponse("Invalid query string. Force value must be a boolean.", currentCorrelationId, loggingName);

                var scopes = _configuration["RumpolePipelineCoordinatorScope"];
                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Getting an access token as part of OBO for the following scope {scopes}");
                var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessTokenAsync(validationResult.AccessTokenValue.ToJwtString(), scopes, currentCorrelationId);

                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Triggering the pipeline for caseId: {caseId}, forceRefresh: {force}");
                await _pipelineClient.TriggerCoordinatorAsync(caseId, onBehalfOfAccessToken, force, currentCorrelationId);

                return new OkObjectResult(_triggerCoordinatorResponseFactory.Create(req, currentCorrelationId));
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    MsalException => InternalServerErrorResponse(exception, "An onBehalfOfToken exception occurred.", currentCorrelationId, loggingName),
                    HttpRequestException => InternalServerErrorResponse(exception, "A pipeline client http exception occurred.", currentCorrelationId, loggingName),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.", currentCorrelationId, loggingName)
                };
            }
            finally
            {
                _logger.LogMethodExit(currentCorrelationId, loggingName, string.Empty);
            }
        }
    }
}


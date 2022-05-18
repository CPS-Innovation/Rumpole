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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineTriggerCoordinator
    {
        private readonly ILogger<RumpolePipelineTriggerCoordinator> _logger;
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly IPipelineClient _pipelineClient;
        private readonly IConfiguration _configuration;
        private readonly ITriggerCoordinatorResponseFactory _triggerCoordinatorResponseFactory;

        public RumpolePipelineTriggerCoordinator(ILogger<RumpolePipelineTriggerCoordinator> logger,
                                 IOnBehalfOfTokenClient onBehalfOfTokenClient,
                                 IPipelineClient pipelineClient,
                                 IConfiguration configuration,
                                 ITriggerCoordinatorResponseFactory triggerCoordinatorResponseFactory)
        {
            _logger = logger;
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _pipelineClient = pipelineClient;
            _configuration = configuration;
            _triggerCoordinatorResponseFactory = triggerCoordinatorResponseFactory;
        }

        [FunctionName("RumpolePipelineTriggerCoordinator")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cases/{caseId}")] HttpRequest request, string caseId)
        {
            try
            {
                string errorMessage;
                if (!request.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                {
                    errorMessage = "Authorization token is not supplied.";
                    return ErrorResponse(new UnauthorizedObjectResult(errorMessage), errorMessage);
                }

                if (!int.TryParse(caseId, out var _))
                {
                    errorMessage = "Invalid case id. A 32-bit integer is required.";
                    return ErrorResponse(new BadRequestObjectResult(errorMessage), errorMessage);
                }

                var force = false;
                if (request.Query.ContainsKey("force") && !bool.TryParse(request.Query["force"], out force))
                {
                    errorMessage = "Invalid query string. Force value must be a boolean.";
                    return ErrorResponse(new BadRequestObjectResult(errorMessage), errorMessage);
                }

                var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessTokenAsync(accessToken.ToJwtString(), _configuration["RumpolePipelineCoordinatorScope"]);

                await _pipelineClient.TriggerCoordinatorAsync(caseId, onBehalfOfAccessToken, force);

                return new OkObjectResult(_triggerCoordinatorResponseFactory.Create(request));
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    MsalException => InternalServerErrorResponse(exception, "An onBehalfOfToken exception occurred."),
                    HttpRequestException => InternalServerErrorResponse(exception, "A pipeline client http exception occurred."),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.")
                };
            }
        }

        private IActionResult ErrorResponse(IActionResult result, string errorMessage)
        {
            _logger.LogError(errorMessage);
            return result;
        }

        private IActionResult InternalServerErrorResponse(Exception exception, string baseErrorMessage)
        {
            _logger.LogError(exception, baseErrorMessage);
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}


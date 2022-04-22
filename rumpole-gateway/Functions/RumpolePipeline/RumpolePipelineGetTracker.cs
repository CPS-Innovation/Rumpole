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

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineGetTracker
    {
        private readonly ILogger<RumpolePipelineGetTracker> _logger;
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly IPipelineClient _pipelineClient;
        private readonly IConfiguration _configuration;

        public RumpolePipelineGetTracker(ILogger<RumpolePipelineGetTracker> logger,
                                 IOnBehalfOfTokenClient onBehalfOfTokenClient,
                                 IPipelineClient pipelineClient,
                                 IConfiguration configuration)
        {
            _logger = logger;
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _pipelineClient = pipelineClient;
            _configuration = configuration;
        }

        [FunctionName("RumpolePipelineGetTracker")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cases/{caseId}/tracker")] HttpRequest req, string caseId)
        {
            try
            {
                string errorMessage;
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                {
                    errorMessage = "Authorization token is not supplied.";
                    return ErrorResponse(new UnauthorizedObjectResult(errorMessage), errorMessage);
                }

                if (!int.TryParse(caseId, out var _))
                {
                    errorMessage = "Invalid case id. A 32-bit integer is required.";
                    return ErrorResponse(new BadRequestObjectResult(errorMessage), errorMessage);
                }

                var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessTokenAsync(accessToken.ToJwtString(), _configuration["RumpolePipelineCoordinatorScope"]);

                var tracker = await _pipelineClient.GetTrackerAsync(caseId, onBehalfOfAccessToken);

                if (tracker == null)
                {
                    errorMessage = $"No tracker found for case id '{caseId}'.";
                    return ErrorResponse(new NotFoundObjectResult(errorMessage), errorMessage);
                }

                return new OkObjectResult(tracker);
            }
            catch(Exception exception)
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

        private IActionResult InternalServerErrorResponse( Exception exception, string baseErrorMessage)
        {
            _logger.LogError(exception, baseErrorMessage);
            return new StatusCodeResult(500);
        }
    }
}


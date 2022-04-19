using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Helpers.Extension;
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

            var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessToken(accessToken.ToJwtString(), _configuration["RumpolePipelineScope"]);

            var tracker = await _pipelineClient.GetTracker(caseId, onBehalfOfAccessToken);

            if(tracker == null)
            {
                return new NotFoundObjectResult($"No tracker found for case id '{caseId}'.");
            }

            return new OkObjectResult(tracker);
        }

        private IActionResult ErrorResponse(IActionResult result, string message)
        {
            _logger.LogError(message);
            return result;
        }
    }
}


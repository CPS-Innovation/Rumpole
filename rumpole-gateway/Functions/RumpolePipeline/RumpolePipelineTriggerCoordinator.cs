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
    public class RumpolePipelineTriggerCoordinator
    {
        private readonly ILogger<RumpolePipelineTriggerCoordinator> _logger;
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly IPipelineClient _pipelineClient;
        private readonly IConfiguration _configuration;

        public RumpolePipelineTriggerCoordinator(ILogger<RumpolePipelineTriggerCoordinator> logger,
                                 IOnBehalfOfTokenClient onBehalfOfTokenClient,
                                 IPipelineClient pipelineClient,
                                 IConfiguration configuration)
        {
            _logger = logger;
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _pipelineClient = pipelineClient;
            _configuration = configuration;
        }

        [FunctionName("RumpolePipelineTriggerCoordinator")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cases/{caseId}")] HttpRequest req, string caseId)
        {
            string errorMsg;
            if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
            {
                errorMsg = "Authorization token is not supplied.";
                return ErrorResponse(new UnauthorizedObjectResult(errorMsg), errorMsg);
            }

            if (!int.TryParse(caseId, out var _))
            {
                errorMsg = "Invalid case id. A 32-bit integer is required.";
                return ErrorResponse(new BadRequestObjectResult(errorMsg), errorMsg);
            }

            var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessToken(accessToken.ToJwtString(), _configuration["RumpolePipelineScope"]);

            var response = await _pipelineClient.TriggerCoordinator(caseId, onBehalfOfAccessToken);

            //TODO test returning http response message works
            return new OkObjectResult(response);
        }

        private IActionResult ErrorResponse(IActionResult result, string message)
        {
            _logger.LogError(message);
            return result;
        }
    }
}


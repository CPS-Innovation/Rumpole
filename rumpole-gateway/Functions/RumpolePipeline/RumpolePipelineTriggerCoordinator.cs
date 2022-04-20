using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Helpers.Extension;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
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
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cases/{caseId}")] HttpRequest req, string caseId)
        {
            string errorMessage;
            if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
            {
                errorMessage = "Authorization token is not supplied.";
                return ErrorResponse(HttpStatusCode.Unauthorized, errorMessage);
            }

            if (!int.TryParse(caseId, out var _))
            {
                errorMessage = "Invalid case id. A 32-bit integer is required.";
                return ErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessToken(accessToken.ToJwtString(), _configuration["RumpolePipelineCoordinatorScope"]);

            return await _pipelineClient.TriggerCoordinator(caseId, onBehalfOfAccessToken);
        }

        private HttpResponseMessage ErrorResponse(HttpStatusCode statusCode, string errorMessage)
        {
            _logger.LogError(errorMessage);

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(errorMessage, Encoding.UTF8, MediaTypeNames.Application.Json)
            };
        }
    }
}


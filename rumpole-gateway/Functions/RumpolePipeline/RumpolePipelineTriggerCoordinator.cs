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
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineTriggerCoordinator : BaseRumpoleFunction
    {
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly IPipelineClient _pipelineClient;
        private readonly IConfiguration _configuration;
        private readonly ITriggerCoordinatorResponseFactory _triggerCoordinatorResponseFactory;
        private readonly IJwtBearerValidator _tokenValidator;

        public RumpolePipelineTriggerCoordinator(ILogger<RumpolePipelineTriggerCoordinator> logger, IOnBehalfOfTokenClient onBehalfOfTokenClient,
                                 IPipelineClient pipelineClient, IConfiguration configuration,
                                 ITriggerCoordinatorResponseFactory triggerCoordinatorResponseFactory, IJwtBearerValidator tokenValidator)
        : base(logger)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _pipelineClient = pipelineClient;
            _configuration = configuration;
            _triggerCoordinatorResponseFactory = triggerCoordinatorResponseFactory;
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
        }

        [FunctionName("RumpolePipelineTriggerCoordinator")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cases/{caseId}")] HttpRequest request, string caseId)
        {
            try
            {
                if (!request.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse();

                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed");

                if (!int.TryParse(caseId, out var _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.");
                
                var force = false;
                if (request.Query.ContainsKey("force") && !bool.TryParse(request.Query["force"], out force))
                    return BadRequestErrorResponse("Invalid query string. Force value must be a boolean.");
                
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
    }
}


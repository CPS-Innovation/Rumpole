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
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineGetTracker : BaseRumpoleFunction
    {
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly IPipelineClient _pipelineClient;
        private readonly IConfiguration _configuration;
        private readonly IJwtBearerValidator _tokenValidator;

        public RumpolePipelineGetTracker(ILogger<RumpolePipelineGetTracker> logger, IOnBehalfOfTokenClient onBehalfOfTokenClient, IPipelineClient pipelineClient,
                                 IConfiguration configuration, IJwtBearerValidator tokenValidator)
        : base(logger)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _pipelineClient = pipelineClient;
            _configuration = configuration;
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
        }

        [FunctionName("RumpolePipelineGetTracker")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cases/{caseId}/tracker")] HttpRequest req, string caseId)
        {
            try
            {
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse();

                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed");

                if (!int.TryParse(caseId, out var _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.");
                
                var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessTokenAsync(accessToken.ToJwtString(), _configuration["RumpolePipelineCoordinatorScope"]);

                var tracker = await _pipelineClient.GetTrackerAsync(caseId, onBehalfOfAccessToken);

                return tracker == null ? NotFoundErrorResponse($"No tracker found for case id '{caseId}'.") : new OkObjectResult(tracker);
            }
            catch(Exception exception)
            {
                return exception switch
                {
                    MsalException => InternalServerErrorResponse(exception, "An onBehalfOfToken exception occurred."),
                    HttpRequestException => InternalServerErrorResponse(exception, "A pipeline client http exception occurred when calling GetTracker."),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.")
                };
            }
        }
    }
}


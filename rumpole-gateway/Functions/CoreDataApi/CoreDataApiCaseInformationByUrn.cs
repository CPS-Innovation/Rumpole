using System;
using System.Linq;
using System.Threading.Tasks;
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

namespace RumpoleGateway.Functions.CoreDataApi
{
    public class CoreDataApiCaseInformationByUrn
    {
        private readonly ILogger<CoreDataApiCaseInformationByUrn> _logger;
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly ICoreDataApiClient _coreDataApiClient;
        private readonly IConfiguration _configuration;

        public CoreDataApiCaseInformationByUrn(ILogger<CoreDataApiCaseInformationByUrn> logger,
                                 IOnBehalfOfTokenClient onBehalfOfTokenClient,
                                 ICoreDataApiClient coreDataApiClient,
                                 IConfiguration configuration)
        {
            _logger = logger;
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _coreDataApiClient = coreDataApiClient;
            _configuration = configuration;
        }

        [FunctionName("CoreDataApiCaseInformationByUrn")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "case-information-by-urn/{urn}")] HttpRequest req,
            string urn)
        {
            try
            {
                string errorMsg;
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                {
                    errorMsg = "Authorization token is not supplied.";
                    return ErrorResponse(new UnauthorizedObjectResult(errorMsg), errorMsg);
                }

                if (string.IsNullOrEmpty(urn))
                {
                    errorMsg = "Urn is not supplied.";
                    return ErrorResponse(new BadRequestObjectResult(errorMsg), errorMsg);
                }

                var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessTokenAsync(accessToken.ToJwtString(), _configuration["CoreDataApiScope"]);

                var caseInformation = await _coreDataApiClient.GetCaseInformationByUrnAsync(urn, onBehalfOfAccessToken);

                if (caseInformation != null && caseInformation.Any())
                {
                    return new OkObjectResult(caseInformation);
                }

                errorMsg = $"No data found for urn '{urn}'.";
                return ErrorResponse(new NotFoundObjectResult(errorMsg), errorMsg);
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    MsalException => InternalServerErrorResponse(exception, "An msal exception occurred."),
                    CoreDataApiException => InternalServerErrorResponse(exception, "A core data api exception occurred."),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.")
                };
            }
        }

        private IActionResult ErrorResponse(IActionResult result, string message)
        {
            _logger.LogError(message);
            return result;
        }

        private IActionResult InternalServerErrorResponse(Exception exception, string baseErrorMessage)
        {
            _logger.LogError(exception, baseErrorMessage);
            return new StatusCodeResult(500);
        }
    }
}


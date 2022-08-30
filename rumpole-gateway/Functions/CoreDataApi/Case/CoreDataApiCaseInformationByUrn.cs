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
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Helpers.Extension;

namespace RumpoleGateway.Functions.CoreDataApi.Case
{
    public class CoreDataApiCaseInformationByUrn : BaseRumpoleFunction
    {
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly ICoreDataApiClient _coreDataApiClient;
        private readonly IConfiguration _configuration;
        private readonly ITokenValidator _tokenValidator;

        public CoreDataApiCaseInformationByUrn(ILogger<CoreDataApiCaseInformationByUrn> logger, IOnBehalfOfTokenClient onBehalfOfTokenClient, ICoreDataApiClient coreDataApiClient,
                                 IConfiguration configuration, ITokenValidator tokenValidator)
        : base(logger)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _coreDataApiClient = coreDataApiClient;
            _configuration = configuration;
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
        }

        [FunctionName("CoreDataApiCaseInformationByUrn")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "case-information-by-urn/{urn}")] HttpRequest req,
            string urn)
        {
            try
            {
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse();

                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed");

                if (string.IsNullOrEmpty(urn))
                    return BadRequestErrorResponse("Urn is not supplied.");
                
                var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessTokenAsync(accessToken.ToJwtString(), _configuration["CoreDataApiScope"]);

                var caseInformation = await _coreDataApiClient.GetCaseInformationByUrnAsync(urn, onBehalfOfAccessToken);

                if (caseInformation != null && caseInformation.Any())
                {
                    return new OkObjectResult(caseInformation);
                }

                return NotFoundErrorResponse($"No data found for urn '{urn}'.");
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
    }
}


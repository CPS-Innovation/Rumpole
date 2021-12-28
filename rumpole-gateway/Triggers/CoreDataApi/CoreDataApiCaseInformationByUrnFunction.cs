using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.CoreDataApi;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Helpers.Extension;

namespace RumpoleGateway.Triggers.CoreDataApi
{
    public class CoreDataApiCaseInformationByUrnFunction
    {
        private readonly ILogger<CoreDataApiCaseInformationByUrnFunction> _logger;
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly ICoreDataApiClient _coreDataApiClient;

        public CoreDataApiCaseInformationByUrnFunction(ILogger<CoreDataApiCaseInformationByUrnFunction> logger,
                                 IOnBehalfOfTokenClient onBehalfOfTokenClient,
                                 ICoreDataApiClient coreDataApiClient)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _logger = logger;
            _coreDataApiClient = coreDataApiClient;
        }

        [FunctionName("CoreDataApiCaseInformationByUrn")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "case-information-by-urn/{urn}")] HttpRequest req,
            string urn)
        {
            _logger.LogInformation("CoreDataApiCaseInformationByUrn - trigger processed a request.");
            
            if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
            {
                return new UnauthorizedObjectResult(Constants.CommonUserMessages.AuthenticationFailedMessage);
            }

            if (string.IsNullOrEmpty(urn))
            {
                return new BadRequestObjectResult(Constants.CommonUserMessages.URNNotSupplied);
            }

            var behalfToken = await _onBehalfOfTokenClient.GetAccessToken(accessToken.ToJwtString());

            var caseInformation = await _coreDataApiClient.GetCaseInformatoinByURN(urn, behalfToken);

            if (caseInformation != null && caseInformation.Any())
            {
                return new OkObjectResult(caseInformation);
            }
            return new NotFoundObjectResult($"{Constants.CommonUserMessages.NoRecordFound} - URN : {urn}");
        }
    }
}


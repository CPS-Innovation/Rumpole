using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.CoreDataApi;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Helpers.Extension;
using System;
using System.Threading.Tasks;

namespace RumpoleGateway.Triggers.CoreDataApi
{
    public class CoreDataApiCaseInformationByUrn
    {
        private readonly ILogger<CoreDataApiCaseDetails> _logger;
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly ICoreDataApiClient _coreDataApiClient;

        public CoreDataApiCaseInformationByUrn(ILogger<CoreDataApiCaseDetails> logger,
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
            _logger.LogInformation("CoreDataApiCaseDetails - trigger processed a request.");
            if (!req.Headers.TryGetValue("Authorization", out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedAccessException("No authorization token supplied.");
            }
            if (string.IsNullOrEmpty(urn))
            {
                _logger.LogError($"Exception - urn not supplied");
                throw new ArgumentException("URN not supplied");
            }
            var behalfToken = await _onBehalfOfTokenClient.GetAccessToken(accessToken.ToJwtString());
            
            var caseInformation = await _coreDataApiClient.GetCaseDetailsById(urn, behalfToken);

            return new OkObjectResult(caseInformation);
        }
    }
}


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
    public class CoreDataApiCaseDetailsFunction
    {
        private readonly ILogger<CoreDataApiCaseDetailsFunction> _logger;
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly ICoreDataApiClient _coreDataApiClient;

        public CoreDataApiCaseDetailsFunction(ILogger<CoreDataApiCaseDetailsFunction> logger,
                                 IOnBehalfOfTokenClient onBehalfOfTokenClient,
                                 ICoreDataApiClient coreDataApiClient)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _logger = logger;
            _coreDataApiClient = coreDataApiClient;
        }

        [FunctionName("CoreDataApiCaseDetails")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "case-details/{case_id}")] HttpRequest req,
            string case_id)
        {
            _logger.LogInformation("CoreDataApiCaseDetails - trigger processed a request.");
            if (!req.Headers.TryGetValue("Authorization", out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedAccessException("No authorization token supplied.");
            }
            if (string.IsNullOrEmpty(case_id))
            {
                _logger.LogError($"Exception - case id not supplied");
                throw new ArgumentException("Case Id not supplied");
            }
            var behalfToken = await _onBehalfOfTokenClient.GetAccessToken(accessToken.ToJwtString());
            
            var caseDetails = await _coreDataApiClient.GetCaseDetailsById(case_id, behalfToken);

            if (caseDetails != null)
            {
                return new OkObjectResult(caseDetails);
            }
            return new NotFoundObjectResult($"No record found - Case id : {case_id}");
        }
    }
}

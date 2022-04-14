using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.CoreDataApi;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Helpers.Extension;
using System.Threading.Tasks;

namespace RumpoleGateway.Functions.CoreDataApi
{
    public class CoreDataApiCaseDetails
    {
        private readonly ILogger<CoreDataApiCaseDetails> _logger;
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly ICoreDataApiClient _coreDataApiClient;

        public CoreDataApiCaseDetails(ILogger<CoreDataApiCaseDetails> logger,
                                 IOnBehalfOfTokenClient onBehalfOfTokenClient,
                                 ICoreDataApiClient coreDataApiClient)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _logger = logger;
            _coreDataApiClient = coreDataApiClient;
        }

        [FunctionName("CoreDataApiCaseDetails")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "case-details/{caseId}")] HttpRequest req,
            string caseId)
        {
            string errorMsg;
            if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
            {
                errorMsg = "Authorization token is not supplied.";
                return ErrorResponse(new UnauthorizedObjectResult(errorMsg), errorMsg);
            }

            if (string.IsNullOrEmpty(caseId))
            {
                errorMsg = "Case Id is not supplied.";
                return ErrorResponse(new BadRequestObjectResult(errorMsg), errorMsg);
            }

            var behalfToken = await _onBehalfOfTokenClient.GetAccessToken(accessToken.ToJwtString());

            var caseDetails = await _coreDataApiClient.GetCaseDetailsById(caseId, behalfToken);

            if (caseDetails != null)
            {
                return new OkObjectResult(caseDetails);
            }

            errorMsg = $"No record found for case id '{caseId}'.";
            return ErrorResponse(new NotFoundObjectResult(errorMsg), errorMsg);
        }

        private IActionResult ErrorResponse(IActionResult result, string message)
        {
            _logger.LogError(message);
            return result;
        }
    }
}


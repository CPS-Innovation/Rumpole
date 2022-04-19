﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public CoreDataApiCaseDetails(ILogger<CoreDataApiCaseDetails> logger,
                                 IOnBehalfOfTokenClient onBehalfOfTokenClient,
                                 ICoreDataApiClient coreDataApiClient,
                                 IConfiguration configuration)
        {
            _logger = logger;
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _coreDataApiClient = coreDataApiClient;
            _configuration = configuration;
        }

        [FunctionName("CoreDataApiCaseDetails")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "case-details/{caseId}")] HttpRequest req, string caseId)
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

            var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessToken(accessToken.ToJwtString(), _configuration["CoreDataApiScope"]);

            var caseDetails = await _coreDataApiClient.GetCaseDetailsById(caseId, onBehalfOfAccessToken);

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


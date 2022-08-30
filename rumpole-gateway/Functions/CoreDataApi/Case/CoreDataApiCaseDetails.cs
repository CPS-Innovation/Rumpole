﻿using Microsoft.AspNetCore.Http;
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
using System;
using System.Threading.Tasks;
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions.CoreDataApi.Case
{
    public class CoreDataApiCaseDetails : BaseRumpoleFunction
    {
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly ICoreDataApiClient _coreDataApiClient;
        private readonly IConfiguration _configuration;

        public CoreDataApiCaseDetails(ILogger<CoreDataApiCaseDetails> logger, IOnBehalfOfTokenClient onBehalfOfTokenClient, ICoreDataApiClient coreDataApiClient,
                                 IConfiguration configuration, ITokenValidator tokenValidator)
        : base(logger, tokenValidator)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _coreDataApiClient = coreDataApiClient;
            _configuration = configuration;
        }

        [FunctionName("CoreDataApiCaseDetails")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "case-details/{caseId}")] HttpRequest req, string caseId)
        {
            try
            {
                var tokenAssessment = await AssessTokenAsync(req);
                if (tokenAssessment is not {Item1: null})
                    return tokenAssessment.Item1;
                
                if (!int.TryParse(caseId, out _))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.");
                
                var onBehalfOfAccessToken = await _onBehalfOfTokenClient.GetAccessTokenAsync(tokenAssessment.Item2.ToJwtString(), _configuration["CoreDataApiScope"]);

                var caseDetails = await _coreDataApiClient.GetCaseDetailsByIdAsync(caseId, onBehalfOfAccessToken);

                return caseDetails != null ? new OkObjectResult(caseDetails) : NotFoundErrorResponse($"No data found for case id '{caseId}'.");
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    MsalException => InternalServerErrorResponse(exception, "An MSAL exception occurred."),
                    CoreDataApiException => InternalServerErrorResponse(exception, "A core data api exception occurred."),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.")
                };
            }
        }
    }
}


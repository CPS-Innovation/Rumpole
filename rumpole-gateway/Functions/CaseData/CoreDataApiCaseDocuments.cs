using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Domain.CoreDataApi;
using RumpoleGateway.Domain.CaseData;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Extensions;
using RumpoleGateway.Services;
using RumpoleGateway.Factories;

namespace RumpoleGateway.Functions.CaseData
{
    public class CoreDataApiCaseDocuments : BaseRumpoleFunction
    {
        private readonly IOnBehalfOfTokenClient _onBehalfOfTokenClient;
        private readonly ICaseDataService _caseDataService;
        private readonly IConfiguration _configuration;
        private readonly ICaseDataArgFactory _caseDataArgFactory;
        private readonly ILogger<CoreDataApiCaseDocuments> _logger;

        public CoreDataApiCaseDocuments(ILogger<CoreDataApiCaseDocuments> logger, IOnBehalfOfTokenClient onBehalfOfTokenClient, ICaseDataService caseDataService,
                                 IConfiguration configuration, IAuthorizationValidator tokenValidator, ICaseDataArgFactory caseDataArgFactory)
        : base(logger, tokenValidator)
        {
            _onBehalfOfTokenClient = onBehalfOfTokenClient;
            _caseDataService = caseDataService;
            _configuration = configuration;
            _caseDataArgFactory = caseDataArgFactory;
            _logger = logger;
        }

        [FunctionName("CoreDataApiCaseDocuments")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "urns/{urn}/cases/{caseId}/documents")] HttpRequest req,
            string urn,
            int? caseId)
        {
            Guid currentCorrelationId = default;
            string upstreamToken = null;
            const string loggingName = "CoreDataApiCaseDocuments - Run";
            IEnumerable<DocumentDetails> documents = null;

            try
            {
                var validationResult = await ValidateRequest(req, loggingName, ValidRoles.UserImpersonation);
                if (validationResult.InvalidResponseResult != null)
                    return validationResult.InvalidResponseResult;

                currentCorrelationId = validationResult.CurrentCorrelationId;
                upstreamToken = validationResult.UpstreamToken;

                _logger.LogMethodEntry(currentCorrelationId, loggingName, string.Empty);

                if (string.IsNullOrEmpty(urn))
                    return BadRequestErrorResponse("Urn is not supplied.", currentCorrelationId, loggingName);

                if (!caseId.HasValue)
                    return BadRequestErrorResponse("CaseId is not supplied.", currentCorrelationId, loggingName);

                // var cdaScope = _configuration[ConfigurationKeys.CoreDataApiScope];
                // _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Getting an access token as part of OBO for the following scope {cdaScope}");
                var onBehalfOfAccessToken = "not-implemented-yet"; //await _onBehalfOfTokenClient.GetAccessTokenAsync(validationResult.AccessTokenValue.ToJwtString(), cdaScope, currentCorrelationId);

                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Getting case documents by Urn '{urn}' and CaseId '{caseId}'");
                documents = await _caseDataService.ListDocuments(_caseDataArgFactory.CreateCaseArg(onBehalfOfAccessToken, upstreamToken, currentCorrelationId, urn, caseId.Value));

                if (documents != null)
                {
                    return new OkObjectResult(documents);
                }

                return NotFoundErrorResponse($"No documents found for Urn '{urn}' and CaseId '{caseId}'.", currentCorrelationId, loggingName);
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    MsalException => InternalServerErrorResponse(exception, "An MSAL exception occurred.", currentCorrelationId, loggingName),
                    CoreDataApiException => InternalServerErrorResponse(exception, "A core data api exception occurred.", currentCorrelationId, loggingName),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.", currentCorrelationId, loggingName)
                };
            }
            finally
            {
                _logger.LogMethodExit(currentCorrelationId, loggingName, documents.ToJson());
            }
        }
    }
}


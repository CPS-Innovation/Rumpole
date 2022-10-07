using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.RumpolePipeline;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineQuerySearchIndex : BaseRumpoleFunction
    {
        private readonly ISearchIndexClient _searchIndexClient;
        private readonly IAuthorizationValidator _tokenValidator;
        private readonly ILogger<RumpolePipelineQuerySearchIndex> _logger;

        public RumpolePipelineQuerySearchIndex(ILogger<RumpolePipelineQuerySearchIndex> logger, ISearchIndexClient searchIndexClient, IAuthorizationValidator tokenValidator) : base(logger)
        {
            _searchIndexClient = searchIndexClient;
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
            _logger = logger;
        }

        [FunctionName("RumpolePipelineQuerySearchIndex")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cases/{caseId}/query/{searchTerm}")] HttpRequest req, string caseId, string searchTerm)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "RumpolePipelineQuerySearchIndex - Run";
            IList<StreamlinedSearchLine> searchResults = null;

            try
            {
                if (!req.Headers.TryGetValue("Correlation-Id", out var correlationId) ||
                    string.IsNullOrWhiteSpace(correlationId))
                    return BadRequestErrorResponse("Invalid correlationId. A valid GUID is required.", currentCorrelationId, loggingName);

                if (!Guid.TryParse(correlationId, out currentCorrelationId))
                    if (currentCorrelationId == Guid.Empty)
                        return BadRequestErrorResponse("Invalid correlationId. A valid GUID is required.", currentCorrelationId, loggingName);

                _logger.LogMethodEntry(currentCorrelationId, loggingName, string.Empty);

                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse(currentCorrelationId, loggingName);

                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken, currentCorrelationId);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed", currentCorrelationId, loggingName);

                if (!int.TryParse(caseId, out var caseIdInt))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.", currentCorrelationId, loggingName);

                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequestErrorResponse("Search term is not supplied.", currentCorrelationId, loggingName);

                searchResults = await _searchIndexClient.Query(caseIdInt, searchTerm, currentCorrelationId);

                return new OkObjectResult(searchResults);
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    RequestFailedException => InternalServerErrorResponse(exception, "A search client index exception occurred.", currentCorrelationId, loggingName),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.", currentCorrelationId, loggingName)
                };
            }
            finally
            {
                _logger.LogMethodExit(currentCorrelationId, loggingName, searchResults.ToJson());
            }
        }
    }
}


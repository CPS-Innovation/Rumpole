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
        private readonly ILogger<RumpolePipelineQuerySearchIndex> _logger;

        public RumpolePipelineQuerySearchIndex(ILogger<RumpolePipelineQuerySearchIndex> logger, ISearchIndexClient searchIndexClient, IAuthorizationValidator tokenValidator) 
            : base(logger, tokenValidator)
        {
            _searchIndexClient = searchIndexClient;
            _logger = logger;
        }

        [FunctionName("RumpolePipelineQuerySearchIndex")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cases/{caseId}/query/{*searchTerm}")] HttpRequest req, string caseId, string searchTerm)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "RumpolePipelineQuerySearchIndex - Run";
            IList<StreamlinedSearchLine> searchResults = null;

            try
            {
                var validationResult = await ValidateRequest(req, loggingName);
                if (validationResult.InvalidResponseResult != null)
                    return validationResult.InvalidResponseResult;
                
                _logger.LogMethodEntry(currentCorrelationId, loggingName, string.Empty);

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


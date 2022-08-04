using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.RumpolePipeline;
using System;
using System.Threading.Tasks;

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineQuerySearchIndex : BaseRumpoleFunction
    {
        private readonly ISearchIndexClient _searchIndexClient;

        public RumpolePipelineQuerySearchIndex(ILogger<RumpolePipelineQuerySearchIndex> logger, ISearchIndexClient searchIndexClient) : base(logger)
        {
            _searchIndexClient = searchIndexClient;
        }

        [FunctionName("RumpolePipelineQuerySearchIndex")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cases/{caseId}/query/{searchTerm}")] HttpRequest req, string caseId, string searchTerm)
        {
            try
            {
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse();

                if (!int.TryParse(caseId, out var caseIdInt))
                    return BadRequestErrorResponse("Invalid case id. A 32-bit integer is required.");
                
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequestErrorResponse("Search term is not supplied.");
                
                var searchResults = await _searchIndexClient.Query(caseIdInt, searchTerm);

                return new OkObjectResult(searchResults);
            }
            catch(Exception exception)
            {
                return exception switch
                {
                    RequestFailedException => InternalServerErrorResponse(exception, "A search index client exception occurred."),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.")
                };
            }
        }
    }
}


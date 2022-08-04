using System;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Services;

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineGetSasUrl : BaseRumpoleFunction
    {
        private readonly ISasGeneratorService _sasGeneratorService;

        public RumpolePipelineGetSasUrl(ILogger<RumpolePipelineGetSasUrl> logger, ISasGeneratorService sasGeneratorService)
            : base(logger)
        {
            _sasGeneratorService = sasGeneratorService;
        }

        [FunctionName("RumpolePipelineGetSasUrl")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pdf/sasUrl/{*blobName}")]
            HttpRequest req, string blobName)
        {
            try
            {
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse();

                if (string.IsNullOrWhiteSpace(blobName))
                    return BadRequestErrorResponse("Blob name is not supplied.");
                
                var sasUrl = await _sasGeneratorService.GenerateSasUrlAsync(blobName);

                return !string.IsNullOrWhiteSpace(sasUrl) ? new OkObjectResult(sasUrl) : NotFoundErrorResponse($"No pdf document found for blob name '{blobName}'.");
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    RequestFailedException => InternalServerErrorResponse(exception, "A blob storage exception occurred."),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.")
                };
            }
        }
    }
}

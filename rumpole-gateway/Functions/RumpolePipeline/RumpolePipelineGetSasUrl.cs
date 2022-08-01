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
    public class RumpolePipelineGetSasUrl
    {
        private readonly ILogger<RumpolePipelineGetSasUrl> _logger;
        private readonly ISasGeneratorService _sasGeneratorService;

        public RumpolePipelineGetSasUrl(ILogger<RumpolePipelineGetSasUrl> logger, ISasGeneratorService sasGeneratorService)
        {
            _logger = logger;
            _sasGeneratorService = sasGeneratorService;
        }

        [FunctionName("RumpolePipelineGetSasUrl")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pdf/sasUrl/{*blobName}")]
            HttpRequest req, string blobName)
        {
            try
            {
                string errorMessage;
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) ||
                    string.IsNullOrWhiteSpace(accessToken))
                {
                    errorMessage = "Authorization token is not supplied.";
                    return ErrorResponse(new UnauthorizedObjectResult(errorMessage), errorMessage);
                }

                if (string.IsNullOrWhiteSpace(blobName))
                {
                    errorMessage = "Blob name is not supplied.";
                    return ErrorResponse(new BadRequestObjectResult(errorMessage), errorMessage);
                }

                var sasUrl = await _sasGeneratorService.GenerateSasUrlAsync(blobName);

                if (!string.IsNullOrWhiteSpace(sasUrl)) return new OkObjectResult(sasUrl);

                errorMessage = $"No pdf document found for blob name '{blobName}'.";
                return ErrorResponse(new NotFoundObjectResult(errorMessage), errorMessage);
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    RequestFailedException => InternalServerErrorResponse(exception,
                        "A blob storage exception occurred."),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.")
                };
            }
        }
    
        private IActionResult ErrorResponse(IActionResult result, string message)
        {
            _logger.LogError(message);
            return result;
        }

        private IActionResult InternalServerErrorResponse(Exception exception, string baseErrorMessage)
        {
            _logger.LogError(exception, baseErrorMessage);
            return new StatusCodeResult(500);
        }
    }
}

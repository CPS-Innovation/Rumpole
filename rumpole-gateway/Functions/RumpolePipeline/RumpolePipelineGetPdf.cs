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
    public class RumpolePipelineGetPdf
    {
        private readonly ILogger<RumpolePipelineGetPdf> _logger;
        private readonly IBlobStorageClient _blobStorageClient;

        public RumpolePipelineGetPdf(ILogger<RumpolePipelineGetPdf> logger, IBlobStorageClient blobStorageClient)
        {
            _logger = logger;
            _blobStorageClient = blobStorageClient;
        }

        [FunctionName("RumpolePipelineGetPdf")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pdfs/{blobName}")] HttpRequest req, string blobName)
        {
            try
            {
                string errorMessage;
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                {
                    errorMessage = "Authorization token is not supplied.";
                    return ErrorResponse(new UnauthorizedObjectResult(errorMessage), errorMessage);
                }

                if (string.IsNullOrWhiteSpace(blobName))
                {
                    errorMessage = "Blob name is not supplied.";
                    return ErrorResponse(new BadRequestObjectResult(errorMessage), errorMessage);
                }

                var blobStream = _blobStorageClient.GetDocumentAsync(blobName);

                if(blobStream == null)
                {
                    errorMessage = $"No pdf found for blob name '{blobName}'.";
                    return ErrorResponse(new NotFoundObjectResult(errorMessage), errorMessage);
                }

                return new OkObjectResult(blobStream);
            }
            catch(Exception exception)
            {
                return exception switch
                {
                    RequestFailedException => InternalServerErrorResponse(exception, "A blob storage exception occurred."),
                    //TODO test for realz
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


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
    public class RumpolePipelineGetPdf : BaseRumpoleFunction
    {
        private readonly IBlobStorageClient _blobStorageClient;

        public RumpolePipelineGetPdf(IBlobStorageClient blobStorageClient, ILogger<RumpolePipelineGetPdf> logger)
        : base(logger)
        {
            _blobStorageClient = blobStorageClient;
        }

        [FunctionName("RumpolePipelineGetPdf")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pdfs/{*blobName}")] HttpRequest req, string blobName)
        {
            try
            {
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse();

                if (string.IsNullOrWhiteSpace(blobName))
                    return BadRequestErrorResponse("Blob name is not supplied.");

                var blobStream = await _blobStorageClient.GetDocumentAsync(blobName);

                return blobStream != null ? new OkObjectResult(blobStream) : NotFoundErrorResponse($"No pdf document found for blob name '{blobName}'.");
            }
            catch(Exception exception)
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


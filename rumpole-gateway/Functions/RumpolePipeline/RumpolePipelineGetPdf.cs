using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.RumpolePipeline;
using System;
using System.Threading.Tasks;
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineGetPdf : BaseRumpoleFunction
    {
        private readonly IBlobStorageClient _blobStorageClient;
        private readonly IAuthorizationValidator _tokenValidator;

        public RumpolePipelineGetPdf(IBlobStorageClient blobStorageClient, ILogger<RumpolePipelineGetPdf> logger, IAuthorizationValidator tokenValidator)
        : base(logger)
        {
            _blobStorageClient = blobStorageClient;
            _tokenValidator = tokenValidator;
        }

        [FunctionName("RumpolePipelineGetPdf")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pdfs/{*blobName}")] HttpRequest req, string blobName)
        {
            try
            {
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse();

                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed");

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


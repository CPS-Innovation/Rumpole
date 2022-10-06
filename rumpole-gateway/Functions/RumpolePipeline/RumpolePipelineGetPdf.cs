using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.RumpolePipeline;
using System;
using System.Threading.Tasks;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineGetPdf : BaseRumpoleFunction
    {
        private readonly IBlobStorageClient _blobStorageClient;
        private readonly IAuthorizationValidator _tokenValidator;
        private readonly ILogger<RumpolePipelineGetPdf> _logger;

        public RumpolePipelineGetPdf(IBlobStorageClient blobStorageClient, ILogger<RumpolePipelineGetPdf> logger, IAuthorizationValidator tokenValidator)
        : base(logger)
        {
            _blobStorageClient = blobStorageClient;
            _tokenValidator = tokenValidator;
            _logger = logger;
        }

        [FunctionName("RumpolePipelineGetPdf")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pdfs/{*blobName}")] HttpRequest req, string blobName)
        {
            Guid currentCorrelationId = default;
            const string loggingName = "RumpolePipelineGetPdf - Run";

            try
            {
                if (!req.Headers.TryGetValue("X-Correlation-ID", out var correlationId) ||
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

                if (string.IsNullOrWhiteSpace(blobName))
                    return BadRequestErrorResponse("Blob name is not supplied.", currentCorrelationId, loggingName);

                _logger.LogMethodFlow(currentCorrelationId, loggingName, $"Getting PDF document from Polaris blob storage for blob named '{blobName}'");
                var blobStream = await _blobStorageClient.GetDocumentAsync(blobName, currentCorrelationId);

                return blobStream != null
                    ? new OkObjectResult(blobStream)
                    : NotFoundErrorResponse($"No pdf document found for blob name '{blobName}'.", currentCorrelationId, loggingName);
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    RequestFailedException => InternalServerErrorResponse(exception, "A blob storage exception occurred.", currentCorrelationId, loggingName),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.", currentCorrelationId, loggingName)
                };
            }
            finally
            {
                _logger.LogMethodExit(currentCorrelationId, loggingName, string.Empty);
            }
        }
    }
}


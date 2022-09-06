using System;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Services;
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions.RumpolePipeline
{
    public class RumpolePipelineGetSasUrl : BaseRumpoleFunction
    {
        private readonly ISasGeneratorService _sasGeneratorService;
        private readonly IJwtBearerValidator _tokenValidator;

        public RumpolePipelineGetSasUrl(IJwtBearerValidator tokenValidator, ILogger<RumpolePipelineGetSasUrl> logger, ISasGeneratorService sasGeneratorService)
            : base(logger)
        {
            _sasGeneratorService = sasGeneratorService ?? throw new ArgumentNullException(nameof(sasGeneratorService));
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
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
                
                var validToken = await _tokenValidator.ValidateTokenAsync(accessToken);
                if (!validToken)
                    return BadRequestErrorResponse("Token validation failed");

                if (string.IsNullOrWhiteSpace(blobName))
                    return BadRequestErrorResponse("Blob name is not supplied.");
                
                var sasUrl = await _sasGeneratorService.GenerateSasUrlAsync(blobName);

                return !string.IsNullOrWhiteSpace(sasUrl) ? new OkObjectResult(sasUrl) : NotFoundErrorResponse($"No pdf document found for blob name '{blobName}'.");
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    RequestFailedException requestFailedException => RequestFailedErrorResponse(requestFailedException, "A blob storage exception occurred."),
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.")
                };
            }
        }
    }
}

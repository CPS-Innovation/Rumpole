using System;
using System.Net.Http;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using RumpoleGateway.Domain.CoreDataApi;

namespace RumpoleGateway.Functions
{
    public abstract class BaseRumpoleFunction
    {
        private readonly ILogger _logger;
        
        protected BaseRumpoleFunction(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected IActionResult AuthorizationErrorResponse()
        {
            const string errorMsg = "Authorization token is not supplied.";
            _logger.LogError(errorMsg);
            return new UnauthorizedObjectResult(errorMsg);
        }

        protected IActionResult BadRequestErrorResponse(string errorMessage)
        {
            _logger.LogError(errorMessage);
            return new BadRequestObjectResult(errorMessage);
        }

        protected IActionResult NotFoundErrorResponse(string errorMessage)
        {
            _logger.LogError(errorMessage);
            return new NotFoundObjectResult(errorMessage);
        }

        protected IActionResult InternalServerErrorResponse(Exception exception, string additionalMessage)
        {
            _logger.LogError(exception, additionalMessage);
            return new ObjectResult(additionalMessage) { StatusCode = 500 };
        }

        protected void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }
    }
}

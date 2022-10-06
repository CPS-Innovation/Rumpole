using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Domain.Logging;

namespace RumpoleGateway.Functions
{
    public abstract class BaseRumpoleFunction
    {
        private readonly ILogger _logger;
        
        protected BaseRumpoleFunction(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected IActionResult AuthorizationErrorResponse(Guid correlationId, string loggerSource)
        {
            const string errorMsg = "Authorization token is not supplied.";
            _logger.LogMethodFlow(correlationId, loggerSource, errorMsg);
            return new UnauthorizedObjectResult(errorMsg);
        }

        protected IActionResult BadRequestErrorResponse(string errorMessage, Guid correlationId, string loggerSource)
        {
            _logger.LogMethodFlow(correlationId, loggerSource, errorMessage);
            return new BadRequestObjectResult(errorMessage);
        }

        protected IActionResult NotFoundErrorResponse(string errorMessage, Guid correlationId, string loggerSource)
        {
            _logger.LogMethodFlow(correlationId, loggerSource, errorMessage);
            return new NotFoundObjectResult(errorMessage);
        }

        protected IActionResult InternalServerErrorResponse(Exception exception, string additionalMessage, Guid correlationId, string loggerSource)
        {
            _logger.LogMethodError(correlationId, loggerSource, additionalMessage, exception);
            return new ObjectResult(additionalMessage) { StatusCode = 500 };
        }

        protected void LogInformation(string message, Guid correlationId, string loggerSource)
        {
            _logger.LogMethodFlow(correlationId, loggerSource, message);
        }
    }
}

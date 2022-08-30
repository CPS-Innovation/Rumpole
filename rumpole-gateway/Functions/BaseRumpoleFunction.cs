using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;
using RumpoleGateway.Domain.Validators;

namespace RumpoleGateway.Functions
{
    public abstract class BaseRumpoleFunction
    {
        private readonly ILogger _logger;
        private readonly ITokenValidator _tokenValidator;
        
        protected BaseRumpoleFunction(ILogger logger, ITokenValidator tokenValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
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
            return new StatusCodeResult(500);
        }

        protected void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        protected async Task<Tuple<IActionResult, StringValues>> AssessTokenAsync(HttpRequest req)
        {
            if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                return new Tuple<IActionResult, StringValues>(AuthorizationErrorResponse(), string.Empty);
            
            var validToken = await _tokenValidator.ValidateTokenAsync(accessToken);
            return !validToken 
                ? new Tuple<IActionResult, StringValues>(BadRequestErrorResponse("Token validation failed"), string.Empty) 
                : new Tuple<IActionResult, StringValues>(null, accessToken);
        }
    }
}

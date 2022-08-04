﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RumpoleGateway.Functions
{
    public abstract class BaseRumpoleFunction
    {
        private readonly ILogger _logger;

        protected BaseRumpoleFunction(ILogger logger)
        {
            _logger = logger;
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
    }
}

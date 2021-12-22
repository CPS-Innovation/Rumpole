using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Net;

namespace RumpoleGateway.Triggers.Status
{
    public class StatusFunction
    {
        private readonly ILogger<StatusFunction> _logger;
        public StatusFunction(ILogger<StatusFunction> logger)
        {
            _logger = logger;
        }


        [FunctionName("Status")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "urn" })]
        [OpenApiSecurity("OpenIdConnect", SecuritySchemeType.OpenIdConnect, Name = "code", In = OpenApiSecurityLocationType.Header)]
        [OpenApiParameter(name: "urn", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **URN** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public IActionResult Run( [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status/{urn}")] HttpRequest req,
                                  string urn)
        {
            try
            {
                _logger.LogInformation(" Status function processed a request.");
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                {
                    return new UnauthorizedObjectResult(Constants.Status.Status.AuthenticationFailedMessage);
                }

                if (!int.TryParse(urn, out var uniqueReferenceNumber))
                {
                    return new BadRequestObjectResult(Constants.Status.Status.URNNotSupplied);
                }

                var response = new Domain.Status.Status();
                if (string.IsNullOrEmpty(urn))
                {
                    response.URN = "0";
                    response.Message = "Successfully accessed";
                }
                else
                {
                    response.URN = urn;
                    response.Message = $"Successfully accessed - URN : {urn}";
                }

                _logger.LogInformation($"Response message :  {response}");

                return new OkObjectResult(response);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Exception - No authorization token supplied.-  {exception}");
                throw new ArgumentException($"Exception - No authorization token supplied.-  {exception}");
            }
            throw new ArgumentException("Query string must contain 'type'.");
        }
    }
}


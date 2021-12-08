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
using System.Threading.Tasks;

namespace RumpoleGateway.Triggers.Status
{
    public class Status
    {
        private readonly ILogger<Status> _logger;
        //public Status( IUserClient userClient, ILogger<Status> logger)
        public Status(ILogger<Status> logger)
        {
            //   _userClient = userClient;
            _logger = logger;
        }


        [FunctionName("Status")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "urn" })]
        [OpenApiSecurity("OpenIdConnect", SecuritySchemeType.OpenIdConnect, Name = "code", In = OpenApiSecurityLocationType.Header)]
        [OpenApiParameter(name: "urn", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **URN** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "status/{urn}")] HttpRequest req,
            string urn)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");
                if (!req.Headers.TryGetValue("Authorization", out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new UnauthorizedAccessException("No authorization token supplied.");
                }

                if (!int.TryParse(urn, out var uniqueReferenceNumber))
                {
                    _logger.LogError($"Exception - urn not supplied");
                    throw new ArgumentException("Unique Reference Number should be numeric");
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


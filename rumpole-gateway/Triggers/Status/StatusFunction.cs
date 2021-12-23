using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

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
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status/{urn}")] HttpRequest req,
                                  string urn)
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

            var response = new Domain.Status.Status
            {
                URN = uniqueReferenceNumber.ToString(),
                Message = $"Successfully has been accessed - URN : {uniqueReferenceNumber}"
            };

            _logger.LogInformation($"Response message :  {response}");

            return new OkObjectResult(response);
        }
    }
}

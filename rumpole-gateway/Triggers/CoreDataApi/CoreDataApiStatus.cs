using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RumpoleGateway.Clients.UserClient;
using Newtonsoft.Json;
using RumpoleGateway.Helpers.Extension;
using System.Net.Http;
using RumpoleGateway.Clients.CoreDataApi;

namespace RumpoleGateway.Triggers.CoreDataApi
{
    public class CoreDataApiStatus
    {


        private readonly ILogger<CoreDataApiStatus> _logger;
        private readonly IUserClient _userClient;
        private readonly HttpClient _httpClient;
        private readonly ICoreDataApiClient _coreDataApiClient;

        public CoreDataApiStatus(ILogger<CoreDataApiStatus> logger,
                                 IHttpClientFactory httpClientFactory,
                                 IUserClient userClient,
                                 ICoreDataApiClient coreDataApiClient)
        {
            _userClient = userClient;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _coreDataApiClient = coreDataApiClient;
        }


        [FunctionName("CoreDataApiStatus")]
        //[OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        //[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "core-data-api-status")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            if (!req.Headers.TryGetValue("Authorization", out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedAccessException("No authorization token supplied.");
            }
            var user = await _userClient.GetUser(accessToken.ToJwtString());
            var response = await _httpClient.GetAsync("https://core-data.dev.cpsdigital.co.uk/status");
            string result = string.Empty;

            if (response.IsSuccessStatusCode)
            {
                  result  = await response.Content.ReadAsStringAsync();
            }

            var caseInformation = _coreDataApiClient.GetCaseInformationByUrn();

        string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? $"Token : {user.UserPrincipalName}  -- {result}"
                : $"Hello, {name}. your toke {user.UserPrincipalName} -- {result}";

            return new OkObjectResult(responseMessage);
        }
    }
}


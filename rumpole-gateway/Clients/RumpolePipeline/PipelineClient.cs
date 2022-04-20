using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Factories;
using RumpoleGateway.Wrappers;

namespace RumpoleGateway.Clients.RumpolePipeline
{
    public class PipelineClient : IPipelineClient
    {
        private readonly IRumpolePipelineRequestFactory _rumpolePipelineRequestFactory;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IJsonConvertWrapper _jsonConvertWrapper;
        private readonly ILogger<IPipelineClient> _logger;

        public PipelineClient(
            IRumpolePipelineRequestFactory rumpolePipelineRequestFactory,
            HttpClient httpClient,
            IConfiguration configuration,
            IJsonConvertWrapper jsonConvertWrapper,
            ILogger<IPipelineClient> logger)
        {
            _rumpolePipelineRequestFactory = rumpolePipelineRequestFactory;
            _httpClient = httpClient;
           _configuration = configuration;
            _jsonConvertWrapper = jsonConvertWrapper;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> TriggerCoordinator(string caseId, string accessToken)
        {
            HttpResponseMessage response;
            try
            {
                response = await GetHttpResponseMessage($"cases/{caseId}?code={_configuration["RumpolePipelineCoordinatorFunctionAppKey"]}", accessToken);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError($"Error response when triggering coordinator. Exception: {exception}");
                throw;
            }

            return response;
        }

        public async Task<Tracker> GetTracker(string caseId, string accessToken)
        {
            HttpResponseMessage response;
            try
            {
                response = await GetHttpResponseMessage($"cases/{caseId}/tracker?code={_configuration["RumpolePipelineFunctionAppKey"]}", accessToken);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError($"Error response when retrieving tracker. Exception: {exception}");

                if(exception.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }

            var stringContent = await response.Content.ReadAsStringAsync();
            return _jsonConvertWrapper.DeserializeObject<Tracker>(stringContent);
        }

        private async Task<HttpResponseMessage> GetHttpResponseMessage(string requestUri, string accessToken)
        {
            var request = _rumpolePipelineRequestFactory.Create(requestUri, accessToken);
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}


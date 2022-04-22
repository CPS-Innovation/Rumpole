using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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

        public PipelineClient(
            IRumpolePipelineRequestFactory rumpolePipelineRequestFactory,
            HttpClient httpClient,
            IConfiguration configuration,
            IJsonConvertWrapper jsonConvertWrapper)
        {
            _rumpolePipelineRequestFactory = rumpolePipelineRequestFactory;
            _httpClient = httpClient;
           _configuration = configuration;
            _jsonConvertWrapper = jsonConvertWrapper;
        }

        public async Task<HttpResponseMessage> TriggerCoordinatorAsync(string caseId, string accessToken)
        {
            return await GetHttpResponseMessage($"cases/{caseId}?code={_configuration["RumpolePipelineCoordinatorFunctionAppKey"]}", accessToken);
        }

        public async Task<Tracker> GetTrackerAsync(string caseId, string accessToken)
        {
            HttpResponseMessage response;
            try
            {
                response = await GetHttpResponseMessage($"cases/{caseId}/tracker?code={_configuration["RumpolePipelineCoordinatorFunctionAppKey"]}", accessToken);
            }
            catch (HttpRequestException exception)
            {
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


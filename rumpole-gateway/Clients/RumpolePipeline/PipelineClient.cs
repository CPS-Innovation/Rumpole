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
        private readonly IPipelineClientRequestFactory _pipelineClientRequestFactory;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IJsonConvertWrapper _jsonConvertWrapper;

        public PipelineClient(
            IPipelineClientRequestFactory pipelineClientRequestFactory,
            HttpClient httpClient,
            IConfiguration configuration,
            IJsonConvertWrapper jsonConvertWrapper)
        {
            _pipelineClientRequestFactory = pipelineClientRequestFactory;
            _httpClient = httpClient;
           _configuration = configuration;
            _jsonConvertWrapper = jsonConvertWrapper;
        }

        public async Task TriggerCoordinatorAsync(string caseId, string accessToken, bool force)
        {
            var forceQuery = force ? "&&force=true" : string.Empty;
            await SendRequestAsync($"cases/{caseId}?code={_configuration["RumpolePipelineCoordinatorFunctionAppKey"]}{forceQuery}", accessToken);
        }

        public async Task<Tracker> GetTrackerAsync(string caseId, string accessToken)
        {
            HttpResponseMessage response;
            try
            {
                response = await SendRequestAsync($"cases/{caseId}/tracker?code={_configuration["RumpolePipelineCoordinatorFunctionAppKey"]}", accessToken);
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

        private async Task<HttpResponseMessage> SendRequestAsync(string requestUri, string accessToken)
        {
            var request = _pipelineClientRequestFactory.Create(requestUri, accessToken);
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}


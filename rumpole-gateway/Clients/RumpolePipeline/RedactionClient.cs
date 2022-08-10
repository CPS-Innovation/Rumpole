using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RumpoleGateway.Domain.DocumentRedaction;
using RumpoleGateway.Factories;
using RumpoleGateway.Wrappers;

namespace RumpoleGateway.Clients.RumpolePipeline
{
    public class RedactionClient : IRedactionClient
    {
        private readonly IPipelineClientRequestFactory _pipelineClientRequestFactory;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IJsonConvertWrapper _jsonConvertWrapper;

        public RedactionClient(IPipelineClientRequestFactory pipelineClientRequestFactory,
            HttpClient httpClient,
            IConfiguration configuration,
            IJsonConvertWrapper jsonConvertWrapper)
        {
            _pipelineClientRequestFactory = pipelineClientRequestFactory;
            _httpClient = httpClient;
            _configuration = configuration;
            _jsonConvertWrapper = jsonConvertWrapper;
        }

        public async Task<RedactPdfResponse> RedactPdfAsync(RedactPdfRequest redactPdfRequest, string accessToken)
        {
            HttpResponseMessage response;
            try
            {
                var requestMessage = new StringContent(_jsonConvertWrapper.SerializeObject(redactPdfRequest), Encoding.UTF8, "application/json");
                response = await SendPutRequestAsync($"redactPdf?code={_configuration["RumpolePipelineRedactPdfFunctionAppKey"]}", accessToken, requestMessage);
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }

            var stringContent = await response.Content.ReadAsStringAsync();
            return _jsonConvertWrapper.DeserializeObject<RedactPdfResponse>(stringContent);
        }

        private async Task<HttpResponseMessage> SendPutRequestAsync(string requestUri, string accessToken, HttpContent requestMessage)
        {
            var request = _pipelineClientRequestFactory.CreatePut(requestUri, accessToken);
            request.Content = requestMessage;
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}

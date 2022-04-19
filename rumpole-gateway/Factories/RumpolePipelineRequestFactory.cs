using System.Net.Http;
using System.Net.Http.Headers;

namespace RumpoleGateway.Factories
{
	public class RumpolePipelineRequestFactory : IRumpolePipelineRequestFactory
	{
        public HttpRequestMessage Create(string requestUri, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue(Constants.Authentication.Bearer, accessToken);
            return request;
        }
    }
}


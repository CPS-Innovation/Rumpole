using System.Net.Http;

namespace RumpoleGateway.Factories
{
	public interface IRumpolePipelineRequestFactory
	{
		HttpRequestMessage Create(string requestUri, string accessToken);
	}
}


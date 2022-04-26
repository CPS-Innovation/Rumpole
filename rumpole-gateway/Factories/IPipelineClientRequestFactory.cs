using System.Net.Http;

namespace RumpoleGateway.Factories
{
	public interface IPipelineClientRequestFactory
	{
		HttpRequestMessage Create(string requestUri, string accessToken);
	}
}


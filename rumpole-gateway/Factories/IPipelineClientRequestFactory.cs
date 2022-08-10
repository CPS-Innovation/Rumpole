using System.Net.Http;

namespace RumpoleGateway.Factories
{
	public interface IPipelineClientRequestFactory
	{
		HttpRequestMessage CreateGet(string requestUri, string accessToken);

		HttpRequestMessage CreatePut(string requestUri, string accessToken);
	}
}


using System;
using Azure;
using Azure.Core.Serialization;
using Azure.Search.Documents;
using Microsoft.Extensions.Options;
using RumpoleGateway.Domain.RumpolePipeline;

namespace RumpoleGateway.Factories
{
	public class SearchClientFactory : ISearchClientFactory
	{
        private readonly SearchIndexOptions _options;

        public SearchClientFactory(IOptions<SearchIndexOptions> options)
        {
            _options = options.Value;
        }

		public SearchClient Create()
        {
            return new SearchClient(
                new Uri(_options.EndpointUrl),
                _options.IndexName,
                new AzureKeyCredential(_options.AuthorizationKey),
                new SearchClientOptions { Serializer = new NewtonsoftJsonObjectSerializer() });
        }
	}
}


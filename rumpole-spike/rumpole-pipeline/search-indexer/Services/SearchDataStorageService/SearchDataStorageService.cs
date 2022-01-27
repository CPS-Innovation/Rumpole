using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System;
using Azure.Search.Documents.Indexes;
using Azure;
using Azure.Search.Documents;
using Azure.Core.Serialization;

namespace Services.SearchDataStorageService
{
    public class SearchDataStorageService
    {
        private readonly SearchDataStorageOptions _storageOptions;
        private readonly SearchDataIndexOptions _indexOptions;
        private readonly CosmosClient _cosmosClient;
        private readonly SearchIndexClient _indexClient;

        public SearchDataStorageService(IOptions<SearchDataStorageOptions> storageOptions, IOptions<SearchDataIndexOptions> indexOptions)
        {
            _storageOptions = storageOptions.Value;
            _indexOptions = indexOptions.Value;
            _cosmosClient = new CosmosClient(_storageOptions.EndpointUrl, _storageOptions.AuthorizationKey, new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                },
                AllowBulkExecution = true
            });

            _indexClient = new SearchIndexClient(
                new Uri(_indexOptions.EndpointUrl),
                new AzureKeyCredential(_indexOptions.AuthorizationKey),
                new SearchClientOptions { Serializer = new NewtonsoftJsonObjectSerializer() });
        }

        public async Task UploadToIndex(int caseId)
        {
            var container = _cosmosClient.GetContainer(_storageOptions.DatabaseName, _storageOptions.ContainerName);
            var query = new QueryDefinition("SELECT * FROM c ");

            var iterator = container.GetItemQueryIterator<SearchLine>(query,
                        requestOptions: new QueryRequestOptions()
                        {
                            PartitionKey = new PartitionKey(caseId)
                        });

            var results = new List<SearchLine>();
            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync();
                results.AddRange(result.Resource);
            }

            var searchClient = _indexClient.GetSearchClient(_indexOptions.IndexName);

            using var indexer = new SearchIndexingBufferedSender<SearchLine>(searchClient, new SearchIndexingBufferedSenderOptions<SearchLine>
            {
                KeyFieldAccessor = searchLine => searchLine.Id
            });

            await indexer.UploadDocumentsAsync(results);
            await indexer.FlushAsync();
        }

    }
}

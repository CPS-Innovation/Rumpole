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
            var container = _cosmosClient.GetContainer(_storageOptions.DatabaseName, "documents");
            var query = new QueryDefinition("SELECT * FROM c ");

            var iterator = container.GetItemQueryIterator<SearchDocument>(query,
                        requestOptions: new QueryRequestOptions()
                        {
                            PartitionKey = new PartitionKey(caseId)
                        });

            var lines = new List<SearchLine>();
            while (iterator.HasMoreResults)
            {
                var results = await iterator.ReadNextAsync();

                foreach (var result in results)
                {
                    foreach (var readResult in result.ReadResults)
                    {
                        lines.AddRange(readResult.Lines.Select((line, index) => new SearchLine
                        {
                            Id = $"{caseId}-{result.Id}-{readResult.Page}-{index}",
                            CaseId = caseId,
                            DocumentId = result.Id,
                            PageIndex = readResult.Page,
                            LineIndex = index,
                            Language = line.Language,
                            BoundingBox = line.BoundingBox,
                            Appearance = line.Appearance,
                            Text = line.Text,
                            Words = line.Words,
                            TransactionId = result.TransactionId
                        }));
                    }
                }
            }

            var searchClient = _indexClient.GetSearchClient(_indexOptions.IndexName);

            using var indexer = new SearchIndexingBufferedSender<SearchLine>(searchClient, new SearchIndexingBufferedSenderOptions<SearchLine>
            {
                KeyFieldAccessor = searchLine => searchLine.Id
            });

            await indexer.UploadDocumentsAsync(lines);
            await indexer.FlushAsync();
        }

    }
}

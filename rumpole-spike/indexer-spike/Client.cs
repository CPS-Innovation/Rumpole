using System.Dynamic;
using System;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Azure.Core.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Azure.Core;

namespace indexer_spike
{
    public static class Client
    {
        private const string indexEndpointUrl = "https://ss-stef-search-service.search.windows.net";
        private const string indexApiKey = "B0A3651B95A844E109780A56DE459C9B";

        private const string cosmosEndpointUrl = "https://cdb-rumpole-search.documents.azure.com:443/";
        private const string cosmosApiKey = "d7GaNtlsiLXLy7hlfEZKAUfbAJ40s4bltuZ7lM9QGOt65CRI3PmFbjNxjrvlcSbPTGIzCBNhi0VUr4SF0FV39w==";

        private static readonly SearchIndexClient _indexClient;

        private static readonly CosmosClient _cosmosClient;
        static Client()
        {

            // Create a new SearchIndexClient
            _indexClient = new SearchIndexClient(
                new Uri(indexEndpointUrl),
                new AzureKeyCredential(indexApiKey),
                new SearchClientOptions { Serializer = new NewtonsoftJsonObjectSerializer() });

            _cosmosClient = new CosmosClient(cosmosEndpointUrl, cosmosApiKey, new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                },
                AllowBulkExecution = true
            });
        }
        public static async Task Run(string[] args)
        {
            // Response<SearchServiceStatistics> stats = await _indexClient.GetServiceStatisticsAsync();
            // Console.WriteLine($"You are using {stats.Value.Counters.IndexCounter.Usage} indexes.");

            var searchClient = _indexClient.GetSearchClient("cosmosdb-index");
            SearchResults<SearchLine> results = await searchClient.SearchAsync<SearchLine>("drink", new SearchOptions
            {
                Filter = "documentId eq 109"
            });

            var searchLinesToAdd = new List<SearchLine>();
            await foreach (SearchResult<SearchLine> result in results.GetResultsAsync())
            {

                var searchLineToAdd = result.Document;
                // Console.WriteLine(JsonConvert.SerializeObject(searchLineToAdd));
                searchLineToAdd.Id = searchLineToAdd.Id + "-996";
                searchLineToAdd.Text = searchLineToAdd.Text + " Stef";
                searchLinesToAdd.Add(searchLineToAdd);
            }

            // await searchClient.UploadDocumentsAsync(searchLinesToAdd);
            var sender = new SearchIndexingBufferedSenderOptions<SearchLine>
            {
                KeyFieldAccessor = searchLine => searchLine.Id
            };

            using var indexer = new SearchIndexingBufferedSender<SearchLine>(searchClient, sender);

            await indexer.UploadDocumentsAsync(searchLinesToAdd);
            await indexer.FlushAsync();
        }

        public static async Task GetCosmosRecords(int caseId)
        {
            var container = _cosmosClient.GetContainer("document-search", "lines");
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

            Console.WriteLine($"Got {results.Count} results");

            var searchClient = _indexClient.GetSearchClient("lines-index");
            // await searchClient.UploadDocumentsAsync(searchLinesToAdd);
            using var indexer = new SearchIndexingBufferedSender<SearchLine>(searchClient, new SearchIndexingBufferedSenderOptions<SearchLine>
            {
                KeyFieldAccessor = searchLine => searchLine.Id
            });

            int i = 0;
            indexer.ActionSent += (e) =>
            {
                Console.WriteLine("Sent " + i++);
                return Task.CompletedTask;
            };

            await indexer.UploadDocumentsAsync(results);
            await indexer.FlushAsync();
            Console.WriteLine($"Indexed");
        }
    }
}
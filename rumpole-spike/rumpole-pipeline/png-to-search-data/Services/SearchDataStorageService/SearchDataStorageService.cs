using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System;

namespace Services.SearchDataStorageService
{
    public class SearchDataStorageService
    {
        private readonly SearchDataStorageOptions _options;
        private readonly CosmosClient _cosmosClient;

        public SearchDataStorageService(IOptions<SearchDataStorageOptions> options)
        {
            _options = options.Value;
            _cosmosClient = new CosmosClient(_options.EndpointUrl, _options.AuthorizationKey, new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                },
                AllowBulkExecution = true
            });
        }

        public async Task StoreResults(AnalyzeResults analyzeresults, int caseId, int documentId, int pageIndex)
        {
            var container = _cosmosClient.GetContainer(_options.DatabaseName, "lines");

            var tasks = new List<Task>();

            var lines = analyzeresults.ReadResults.First().Lines;
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                var searchLine = new SearchLine
                {
                    Id = $"{caseId}-{documentId}-{pageIndex}-{i}",
                    CaseId = caseId,
                    DocumentId = documentId,
                    PageIndex = pageIndex,
                    LineIndex = i,
                    Language = line.Language,
                    BoundingBox = line.BoundingBox,
                    Appearance = line.Appearance,
                    Text = line.Text,
                    Words = line.Words
                };
                tasks.Add(Upsert(container, searchLine, caseId));
            }

            await Task.WhenAll(tasks);
        }

        private Task Upsert<T>(Container container, T item, int caseId)
        {
            return container.UpsertItemAsync(item, new PartitionKey(caseId)).ContinueWith(itemResponse =>
        {
            if (!itemResponse.IsCompletedSuccessfully)
            {
                AggregateException innerExceptions = itemResponse.Exception.Flatten();
                if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
                {
                    Console.WriteLine($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
                }
                else
                {
                    Console.WriteLine($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
                }
            }
        });
        }
    }
}

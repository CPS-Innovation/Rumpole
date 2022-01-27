using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Services.SearchDataStorageService;

namespace Functions.ProcessDocument
{
    public class PngToSearchData
    {
        private readonly SearchDataStorageService _searchDataStorageService;

        public PngToSearchData(SearchDataStorageService searchDataStorageService)
        {
            _searchDataStorageService = searchDataStorageService;
        }

        [FunctionName("search-indexer")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req, ILogger log)
        {
            var content = await req.Content.ReadAsStringAsync();
            var arg = JsonConvert.DeserializeObject<SearchIndexerArg>(content);

            await _searchDataStorageService.UploadToIndex(arg.CaseId);
        }
    }
}
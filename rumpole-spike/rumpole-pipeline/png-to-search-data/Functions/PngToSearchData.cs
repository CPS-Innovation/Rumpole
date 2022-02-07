using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.OcrService;
using Services.SearchDataStorageService;

namespace Functions.ProcessDocument
{
    public class PngToSearchData
    {
        private readonly OcrService _ocrService;

        private readonly SearchDataStorageService _searchDataStorageService;

        public PngToSearchData(OcrService ocrService, SearchDataStorageService searchDataStorageService)
        {
            _ocrService = ocrService;
            _searchDataStorageService = searchDataStorageService;
        }

        [FunctionName("png-to-search-data")]
        public async Task<PngToSearchDataResponse> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req, ILogger log)
        {
            var content = await req.Content.ReadAsStringAsync();
            var arg = JsonConvert.DeserializeObject<PngToSearchDataArg>(content);

            var url = arg.SasLink;
            var pagePngAnalysisResults = await _ocrService.GetOcrResults(url);
            await _searchDataStorageService.StoreResults(pagePngAnalysisResults, arg.CaseId, arg.DocumentId, arg.PageIndex, arg.TransactionId);

            var thisPageResult = pagePngAnalysisResults.ReadResults[0]; // only one page per png
            return new PngToSearchDataResponse
            {
                PageIndex = arg.PageIndex,
                Height = thisPageResult.Height,
                Width = thisPageResult.Width
            };
        }
    }
}
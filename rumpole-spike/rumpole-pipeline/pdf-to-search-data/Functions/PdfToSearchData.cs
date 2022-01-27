using System.Collections.Generic;
using System.Linq;
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
    public class PdfToSearchData
    {
        private readonly OcrService _ocrService;

        private readonly SearchDataStorageService _searchDataStorageService;

        public PdfToSearchData(OcrService ocrService, SearchDataStorageService searchDataStorageService)
        {
            _ocrService = ocrService;
            _searchDataStorageService = searchDataStorageService;
        }

        [FunctionName("pdf-to-search-data")]
        public async Task<List<PdfToSearchDataResponse>> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req, ILogger log)
        {
            var content = await req.Content.ReadAsStringAsync();
            var arg = JsonConvert.DeserializeObject<PdfToSearchDataArg>(content);

            var url = arg.SasLink;
            var pagePngAnalysisResults = await _ocrService.GetOcrResults(url);
            await _searchDataStorageService.StoreResults(pagePngAnalysisResults, arg.CaseId, arg.DocumentId, arg.TransactionId);

            return pagePngAnalysisResults.ReadResults.Select(readResult => new PdfToSearchDataResponse
            {
                PageIndex = readResult.Page,
                Height = readResult.Height,
                Width = readResult.Width
            }).ToList();
        }
    }
}
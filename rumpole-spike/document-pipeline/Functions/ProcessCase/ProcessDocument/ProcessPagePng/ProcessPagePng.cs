using System.Threading.Tasks;
using Domain;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Services.OcrService;
using Services.SearchDataStorageService;

namespace Functions.ProcessDocument.ProcessPng
{
    public class ProcessPagePng
    {
        private readonly OcrService _ocrService;

        private readonly SearchDataStorageService _searchDataStorageService;

        public ProcessPagePng(OcrService ocrService, SearchDataStorageService searchDataStorageService)
        {
            _ocrService = ocrService;
            _searchDataStorageService = searchDataStorageService;
        }

        [FunctionName("ProcessPagePng")]
        public async Task<AnalyzeResults> Run([ActivityTrigger] ProcessPagePngArgs arg, ILogger log)
        {
            var url = arg.Url;

            log.LogInformation($"Getting OCR results for {url}");
            var pagePngAnalysisResults = await _ocrService.GetOcrResults(url);
            log.LogInformation($"Got OCR results for {url}");
            await _searchDataStorageService.StoreResults(pagePngAnalysisResults, arg.CaseId, arg.DocumentId, arg.PageIndex);
            log.LogInformation($"Stored results for {url}");
            return pagePngAnalysisResults;
        }
    }
}
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Services.OcrService;

namespace Functions.ProcessDocument.ProcessPng
{
  public class ProcessPagePng
  {
    private readonly OcrService _ocrService;

    public ProcessPagePng(OcrService ocrService)
    {
      _ocrService = ocrService;
    }

    [FunctionName("ProcessPagePng")]
    public async Task<AnalyzeResults> Run([ActivityTrigger] string url, ILogger log)
    {
      log.LogInformation($"Getting OCR results for {url}");
      var result = await _ocrService.GetOcrResults(url);
      log.LogInformation($"Got OCR results for {url}");
      return result;
    }
  }
}
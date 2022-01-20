using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions.ProcessDocument
{
  public static class ProcessDocumentOrchestration
  {


    [FunctionName("ProcessDocumentOrchestration")]
    public static async Task<string> RunOrchestrator(
    [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
      var caseDocument = context.GetInput<CmsCaseDocumentDetails>();

      var arg = new TransformAndSplitArg
      {
        CaseId = caseDocument.CaseId,
        DocumentId = caseDocument.Id,
        DocumentUrl = caseDocument.Url
      };

      var transformToPdfResult = await context.CallActivityAsync<TransformAndSplitResult>("TransformAndSplit", arg);

      var pngProcessingTasks = new List<Task>();

      foreach (var pngDetails in transformToPdfResult.Pngs)
      {
        pngProcessingTasks.Add(context.CallActivityAsync<AnalyzeResults>("ProcessPagePng", pngDetails.SasLinkUrl));

      }

      await Task.WhenAll(pngProcessingTasks);

      return pngProcessingTasks.Count.ToString();
    }
  }
}
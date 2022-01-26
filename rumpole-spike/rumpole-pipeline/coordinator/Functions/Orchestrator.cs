using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Functions
{
    public class Orchestrator
    {
        private readonly EndpointOptions endpoints;

        public Orchestrator(IOptions<EndpointOptions> endpointOptions)
        {
            this.endpoints = endpointOptions.Value;
        }

        [FunctionName("Orchestrator_HttpStart")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "cases/{caseId}")] HttpRequestMessage req,
            int caseId,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            string instanceId = await client.StartNewAsync("CaseOrchestration", null, new OrchestratorArg
            {
                CaseId = caseId,
                RequestUrl = req.RequestUri.AbsoluteUri
            });
            return client.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("CaseOrchestration")]
        public async Task RunCaseOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var arg = context.GetInput<OrchestratorArg>();
            var caseId = arg.CaseId;
            var trackerUrl = arg.RequestUrl + "/tracker"; //  todo: not safe if there is a querystring
            context.SetCustomStatus(trackerUrl);

            var tracker = GetTracker(context, caseId);
            await tracker.Initialise();
            var cmsCaseDocumentDetails = await CallHttpAsync<List<CmsCaseDocumentDetails>>(context, HttpMethod.Get, endpoints.CmsDocumentDetails);
            await tracker.Register(cmsCaseDocumentDetails.Select(item => item.Id).ToList());

            var tasks = new List<Task<string>>();
            foreach (var caseDocumentDetails in cmsCaseDocumentDetails)
            {
                caseDocumentDetails.CaseId = caseId;
                tasks.Add(context.CallSubOrchestratorAsync<string>("CaseDocumentOrchestration", caseDocumentDetails));
                //break;
            }

            await Task.WhenAll(tasks);
            tracker.RegisterIsIndexed();
        }

        [FunctionName("CaseDocumentOrchestration")]
        public async Task RunDocumentOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var caseDocument = context.GetInput<CmsCaseDocumentDetails>();
            var caseId = caseDocument.CaseId;
            var documentId = caseDocument.Id;

            var tracker = GetTracker(context, caseId);

            var pdfBlobNameAndSasLinkUrl = await CallHttpAsync<BlobNameAndSasLinkUrl>(context, HttpMethod.Post, endpoints.DocToPdf, new DocToPdfArg
            {
                CaseId = caseId,
                DocumentId = documentId,
                DocumentUrl = caseDocument.Url
            });
            tracker.RegisterPdfUrl(new TrackerPdfArg
            {
                DocumentId = documentId,
                PdfUrl = pdfBlobNameAndSasLinkUrl.SasLinkUrl
            });

            var pngBlobNameAndSasLinkUrls = await CallHttpAsync<List<BlobNameAndSasLinkUrl>>(context, HttpMethod.Post, endpoints.PdfToPng, new PdfToPngsArg
            {
                CaseId = caseId,
                DocumentId = documentId,
                BlobName = pdfBlobNameAndSasLinkUrl.BlobName
            });
            tracker.RegisterPngUrls(new TrackerPngsArg
            {
                DocumentId = documentId,
                PngUrls = pngBlobNameAndSasLinkUrls.Select(item => item.SasLinkUrl).ToList()
            });

            var pngProcessingTasks = new List<Task<PngToSearchDataResponse>>();

            for (int i = 0; i < pngBlobNameAndSasLinkUrls.Count; i++)
            {
                pngProcessingTasks.Add(CallHttpAsync<PngToSearchDataResponse>(context, HttpMethod.Post, endpoints.PngToSearchData, new PngToSearchDataArg
                {
                    CaseId = caseId,
                    DocumentId = documentId,
                    SasLink = pngBlobNameAndSasLinkUrls[i].SasLinkUrl,
                    PageIndex = i
                }));
            }

            var pngToSearchDataResponses = await Task.WhenAll(pngProcessingTasks);

            tracker.RegisterIsProcessedForSearch(new TrackerSearchArg
            {
                DocumentId = documentId,
                PngDimensions = pngToSearchDataResponses
                                    .OrderBy(item => item.PageIndex)
                                    .Select(item => new TrackerPngDimensions
                                    {
                                        Height = item.Height,
                                        Width = item.Width
                                    }).ToList()
            });
        }

        [FunctionName("CaseIndexerOrchestration")]
        public async Task RunIndexerOrchestrator(
                [OrchestrationTrigger] IDurableOrchestrationContext context)
        {

        }


        private async Task<T> CallHttpAsync<T>(IDurableOrchestrationContext context, HttpMethod httpMethod, string url, object arg = null)
        {
            var response = await context.CallHttpAsync(httpMethod, new Uri(url), arg == null ? null : JsonConvert.SerializeObject(arg));
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        private ITracker GetTracker(IDurableOrchestrationContext context, int caseId)
        {
            var entityId = new EntityId(nameof(Tracker), caseId.ToString());
            return context.CreateEntityProxy<ITracker>(entityId);
        }
    }
}
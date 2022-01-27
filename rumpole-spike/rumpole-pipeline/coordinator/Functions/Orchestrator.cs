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
        private readonly EndpointOptions _endpoints;

        public Orchestrator(IOptions<EndpointOptions> endpointOptions)
        {
            this._endpoints = endpointOptions.Value;
        }

        [FunctionName("Orchestrator_HttpStart")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "cases/{caseId}")] HttpRequestMessage req,
            int caseId,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            string instanceId = await client.StartNewAsync("CaseOrchestration", null, new OrchestratorArg
            {
                CaseId = caseId,
                TrackerUrl = $"{req.RequestUri.GetLeftPart(UriPartial.Path)}/tracker{req.RequestUri.Query}"
            });
            return client.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("CaseOrchestration")]
        public async Task<List<TrackerDocument>> RunCaseOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var arg = context.GetInput<OrchestratorArg>();
            var caseId = arg.CaseId;
            var transactionId = context.InstanceId;

            var tracker = GetTracker(context, caseId);
            await tracker.Initialise(transactionId);

            var cmsCaseDocumentDetails = await CallHttpAsync<List<CmsCaseDocumentDetails>>(context, HttpMethod.Get, _endpoints.CmsDocumentDetails);
            await tracker.Register(cmsCaseDocumentDetails.Select(item => item.Id).ToList());

            var caseDocumentTasks = new List<Task<string>>();
            foreach (var caseDocumentDetails in cmsCaseDocumentDetails)
            {
                caseDocumentDetails.CaseId = caseId;
                caseDocumentDetails.TransactionId = transactionId;
                caseDocumentTasks.Add(context.CallSubOrchestratorAsync<string>("CaseDocumentOrchestration", caseDocumentDetails));
                //break;
            }

            await Task.WhenAll(caseDocumentTasks);

            if (_endpoints.SearchIndexerEnabled)
            {
                await CallHttpAsync<object>(context, HttpMethod.Post, _endpoints.SearchIndexer, new SearchIndexerArg
                {
                    CaseId = caseId,
                    TransactionId = transactionId
                });
            }

            tracker.RegisterIsIndexed();

            return await tracker.GetDocuments();
        }

        [FunctionName("CaseDocumentOrchestration")]
        public async Task RunDocumentOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var caseDocument = context.GetInput<CmsCaseDocumentDetails>();
            var caseId = caseDocument.CaseId;
            var documentId = caseDocument.Id;
            var transactionId = caseDocument.TransactionId;

            var tracker = GetTracker(context, caseId);

            var pdfBlobNameAndSasLinkUrl = await CallHttpAsync<BlobNameAndSasLinkUrl>(context, HttpMethod.Post, _endpoints.DocToPdf, new DocToPdfArg
            {
                CaseId = caseId,
                DocumentId = documentId,
                DocumentUrl = caseDocument.Url,
                TransactionId = transactionId
            });
            tracker.RegisterPdfUrl(new TrackerPdfArg
            {
                DocumentId = documentId,
                PdfUrl = pdfBlobNameAndSasLinkUrl.SasLinkUrl
            });

            var pngBlobNameAndSasLinkUrls = await CallHttpAsync<List<BlobNameAndSasLinkUrl>>(context, HttpMethod.Post, _endpoints.PdfToPng, new PdfToPngsArg
            {
                CaseId = caseId,
                DocumentId = documentId,
                BlobName = pdfBlobNameAndSasLinkUrl.BlobName,
                TransactionId = transactionId
            });
            tracker.RegisterPngUrls(new TrackerPngsArg
            {
                DocumentId = documentId,
                PngUrls = pngBlobNameAndSasLinkUrls.Select(item => item.SasLinkUrl).ToList()
            });

            var pngProcessingTasks = new List<Task<PngToSearchDataResponse>>();

            for (int i = 0; i < pngBlobNameAndSasLinkUrls.Count; i++)
            {
                pngProcessingTasks.Add(CallHttpAsync<PngToSearchDataResponse>(context, HttpMethod.Post, _endpoints.PngToSearchData, new PngToSearchDataArg
                {
                    CaseId = caseId,
                    DocumentId = documentId,
                    SasLink = pngBlobNameAndSasLinkUrls[i].SasLinkUrl,
                    PageIndex = i,
                    TransactionId = transactionId
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
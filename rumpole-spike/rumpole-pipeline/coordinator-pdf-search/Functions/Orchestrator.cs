using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var instanceId = caseId.ToString();
            // Check if an instance with the specified ID already exists or an existing one stopped running(completed/failed/terminated).
            var existingInstance = await client.GetStatusAsync(instanceId);
            if (existingInstance == null
            || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Completed
            || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Failed
            || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Terminated)
            {
                var query = System.Web.HttpUtility.ParseQueryString(req.RequestUri.Query);
                var force = query.Get("force");

                await client.StartNewAsync("CaseOrchestration", instanceId, new OrchestratorArg
                {
                    CaseId = caseId,
                    TrackerUrl = $"{req.RequestUri.GetLeftPart(UriPartial.Path)}/tracker{req.RequestUri.Query}",
                    ForceRefresh = force == "true"
                });
            }

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

            if (!arg.ForceRefresh && await tracker.GetIsAlreadyProcessed())
            {
                return await tracker.GetDocuments();
            }

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

            var pdfSearchTask = CallHttpAsync<List<PdfToSearchDataResponse>>(context, HttpMethod.Post, _endpoints.PdfToSearchData, new PdfToSearchDataArg
            {
                CaseId = caseId,
                DocumentId = documentId,
                SasLink = pdfBlobNameAndSasLinkUrl.SasLinkUrl,
                TransactionId = transactionId
            });

            var splitPngsTask = context.CallSubOrchestratorAsync("CaseDocumentSplitPngsOrchestration", new RunDocumentSplitToPngsArg
            {
                CaseId = caseId,
                Id = documentId,
                BlobName = pdfBlobNameAndSasLinkUrl.BlobName,
                TransactionId = transactionId
            });

            var searchTask = context.CallSubOrchestratorAsync("CaseDocumentSearchPdfOrchestration", new PdfToSearchDataArg
            {
                CaseId = caseId,
                DocumentId = documentId,
                SasLink = pdfBlobNameAndSasLinkUrl.SasLinkUrl,
                TransactionId = transactionId
            });

            await Task.WhenAll(splitPngsTask, pdfSearchTask);
        }


        [FunctionName("CaseDocumentSplitPngsOrchestration")]
        public async Task RunDocumentSplitToPngs(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var arg = context.GetInput<RunDocumentSplitToPngsArg>();

            var pngBlobNameAndSasLinkUrls = await CallHttpAsync<List<BlobNameAndSasLinkUrl>>(context, HttpMethod.Post, _endpoints.PdfToPng, new PdfToPngsArg
            {
                CaseId = arg.CaseId,
                DocumentId = arg.Id,
                BlobName = arg.BlobName,
                TransactionId = arg.TransactionId
            });
            var tracker = GetTracker(context, arg.CaseId);
            tracker.RegisterPngUrls(new TrackerPngsArg
            {
                DocumentId = arg.Id,
                PngUrls = pngBlobNameAndSasLinkUrls.Select(item => item.SasLinkUrl).ToList()
            });
        }

        [FunctionName("CaseDocumentSearchPdfOrchestration")]
        public async Task RunDocumentSearchPdf(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var arg = context.GetInput<PdfToSearchDataArg>();

            var response = await CallHttpAsync<List<PdfToSearchDataResponse>>(context, HttpMethod.Post, _endpoints.PdfToSearchData, arg);
            var tracker = GetTracker(context, arg.CaseId);

            tracker.RegisterIsProcessedForSearchAndPngDimensions(new TrackerPngArg
            {
                DocumentId = arg.DocumentId,
                PngDimensions = response
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
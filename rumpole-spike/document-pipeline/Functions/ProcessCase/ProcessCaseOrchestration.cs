using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions
{
    public class ProcessCaseOrchestration
    {

        [FunctionName("Orchestration_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "cases/{caseId}")] HttpRequestMessage req,
        string caseId,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("ProcessCaseOrchestration", null, caseId);

            log.LogInformation($"Started orchestration with ID = '{instanceId}', for case Id {caseId}");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("ProcessCaseOrchestration")]
        public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var caseId = context.GetInput<string>();

            var caseDocumentDetails = await context.CallActivityAsync<List<CmsCaseDocumentDetails>>("GetCaseDocumentDetails", int.Parse(caseId));

            var tasks = new List<Task<string>>();
            foreach (var caseDocument in caseDocumentDetails)
            {
                Task<string> provisionTask = context.CallSubOrchestratorAsync<string>("ProcessDocumentOrchestration", caseDocument);
                tasks.Add(provisionTask);
                //break;
            }

            var strings = await Task.WhenAll(tasks);

            return strings.ToList();
        }
    }
}
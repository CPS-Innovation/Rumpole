using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;

namespace Domain
{
    public class SearchDocument : AnalyzeResults
    {
        [JsonProperty(PropertyName = "caseId")]
        public int CaseId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }
    }
}
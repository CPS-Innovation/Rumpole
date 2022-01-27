using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;

namespace indexer_spike
{
    public class SearchLine : Line
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "caseId")]
        public int CaseId { get; set; }

        [JsonProperty(PropertyName = "documentId")]
        public int DocumentId { get; set; }

        [JsonProperty(PropertyName = "pageIndex")]
        public int PageIndex { get; set; }

        [JsonProperty(PropertyName = "lineIndex")]
        public int LineIndex { get; set; }
    }
}
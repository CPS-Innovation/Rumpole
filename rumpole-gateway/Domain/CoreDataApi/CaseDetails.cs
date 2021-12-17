using Newtonsoft.Json;

namespace RumpoleGateway.Domain.CoreDataApi
{
    public class CaseDetails
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("uniqueReferenceNumber")]
        public string UniqueReferenceNumber { get; set; }

        [JsonProperty("appealType")]
        public string AppealType { get; set; }

        [JsonProperty("caseType")]
        public string CaseType { get; set; }
         

    }
}

using Newtonsoft.Json;

namespace RumpoleGateway.Domain.CoreDataApi.CaseDetails
{
    public class CaseStatus
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}

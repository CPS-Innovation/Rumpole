using Newtonsoft.Json;

namespace RumpoleGateway.Domain.CoreDataApi.ResponseTypes
{
    public class ResponseCaseInformation
    {
        [JsonProperty("case")]
        public CaseDetails CaseDetails { get; set; }
    }
}

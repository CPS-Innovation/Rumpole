using Newtonsoft.Json;

namespace RumpoleGateway.Domain.CoreDataApi.ResponseTypes
{
    public class ResponseCaseDetails
    {
        [JsonProperty("case")]
        public CaseDetails.CaseDetails CaseDetails { get; set; }
    }
}

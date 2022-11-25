using Newtonsoft.Json;

namespace RumpoleGateway.Domain.CaseData
{
    public class HeadlineCharge
    {
        [JsonProperty("charge")]
        public string Charge { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }
    }
}

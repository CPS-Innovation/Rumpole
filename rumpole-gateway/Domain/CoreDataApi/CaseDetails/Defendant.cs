using System.Collections.Generic;
using Newtonsoft.Json;

namespace RumpoleGateway.Domain.CoreDataApi.CaseDetails
{
    public class Defendant
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("defendantDetails")]
        public DefendantDetails DefendantDetails { get; set; }

        [JsonProperty("custodyTimeLimit")]
        public CustodyTimeLimit CustodyTimeLimit { get; set; }

        [JsonProperty("charges")]
        public IEnumerable<Charge> Charges { get; set; }
    }
}

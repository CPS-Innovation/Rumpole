using Newtonsoft.Json;
using System.Collections.Generic;

namespace RumpoleGateway.Domain.CaseData
{
    public class CaseDetailsFull : CaseDetails
    {
        [JsonProperty("defendants")]
        public IEnumerable<DefendantDetails> Defendants { get; set; }
    }
}

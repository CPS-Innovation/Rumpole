using Newtonsoft.Json;
using System.Collections.Generic;

namespace RumpoleGateway.Domain.CoreDataApi.CaseDetails
{
    public class CaseDetailsFull : CaseDetails
    {
        [JsonProperty("defendants")]
        public IEnumerable<DefendantDetails> Defendants { get; set; }
    }
}

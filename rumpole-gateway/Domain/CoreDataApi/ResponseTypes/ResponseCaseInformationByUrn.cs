using Newtonsoft.Json;
using System.Collections.Generic;

namespace RumpoleGateway.Domain.CoreDataApi.ResponseTypes
{
    public class ResponseCaseInformationByUrn
    {
        [JsonProperty("cases")]
        public  List<CaseDetails.CaseDetails> CaseDetails { get; set; }
    }
}

using Newtonsoft.Json;

namespace RumpoleGateway.Domain.CoreDataApi.CaseDetails
{
    public class LeadDefendant
    {
        [JsonProperty("firstNames")]
        public string  FirstNames { get; set; }

        [JsonProperty("surname")]
        public string  Surname { get; set; }

        [JsonProperty("organisationName")]
        public string OrganisationName { get; set; }
    }
}

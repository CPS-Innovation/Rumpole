using Newtonsoft.Json;

namespace RumpoleGateway.Domain.CaseData
{
    public class DefendantDetails
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("listOrder")]
        public string ListOrder { get; set; }

        [JsonProperty("firstNames")]
        public string FirstNames { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("organisationName")]
        public string OrganisationName { get; set; }

        [JsonProperty("dob")]
        public string Dob { get; set; }

        [JsonProperty("youth")]
        public bool isYouth { get; set; }
    }
}

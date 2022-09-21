using System.Collections.Generic;
using Newtonsoft.Json;

namespace RumpoleGateway.Domain.RumpolePipeline
{
    public class StreamlinedWord
    {
        [JsonProperty(PropertyName = "boundingBox")]
        public IList<double?> BoundingBox { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "weighting")]
        public int Weighting { get; set; }
        
        [JsonProperty(PropertyName = "matchType")]
        public StreamlinedMatchType StreamlinedMatchType { get; set; }
    }
}

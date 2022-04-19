using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RumpoleGateway.Domain.RumpolePipeline
{
	public class Tracker
	{
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("documents")]
        public List<TrackerDocument> Documents { get; set; }

        [JsonProperty("logs")]
        public List<Log> Logs { get; set; }

        [JsonProperty("IsComplete")]
        public bool IsComplete { get; set; }
    }
}


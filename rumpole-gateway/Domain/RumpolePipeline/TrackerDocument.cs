using Newtonsoft.Json;

namespace RumpoleGateway.Domain.RumpolePipeline
{
	public class TrackerDocument
	{
        [JsonProperty("documentId")]
        public int DocumentId { get; set; }

        [JsonProperty("pdfBlobName")]
        public string PdfBlobName { get; set; }
    }
}


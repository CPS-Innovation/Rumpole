using System.Collections.Generic;

namespace RumpoleGateway.Domain.DocumentRedaction
{
    public class RedactPdfRequest
    {
        public string CaseId { get; set; }

        public string DocumentId { get; set; }

        public string FileName { get; set; }

        public List<RedactionDefinition> RedactionDefinitions { get; set; }
    }
}

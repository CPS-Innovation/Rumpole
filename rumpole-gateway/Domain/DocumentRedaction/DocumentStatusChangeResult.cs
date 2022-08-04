namespace RumpoleGateway.Domain.DocumentRedaction
{
    public class DocumentStatusChangeResult
    {
        public DocumentStatusChangeResult(bool successful, DocumentRedactionStatus documentStatus)
        {
            Successful = successful;
            DocumentStatus = documentStatus;
        }

        public bool Successful { get; set; }
        
        public DocumentRedactionStatus DocumentStatus { get; set; }
    }
}

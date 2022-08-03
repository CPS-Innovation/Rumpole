namespace RumpoleGateway.Domain.DocumentRedaction
{
    public class DocumentCheckOutResult
    {
        public DocumentCheckOutResult(bool successful, DocumentCheckOutStatus documentCheckOutStatus)
        {
            Successful = successful;
            DocumentCheckOutStatus = documentCheckOutStatus;
        }

        public bool Successful { get; set; }
        
        public DocumentCheckOutStatus DocumentCheckOutStatus { get; set; }
    }
}

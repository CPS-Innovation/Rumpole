namespace Domain
{
    public class RunDocumentSplitToPngsArg
    {
        public int CaseId { get; set; }
        public int Id { get; set; }
        public string BlobName { get; set; }
        public string TransactionId { get; set; }
    }
}
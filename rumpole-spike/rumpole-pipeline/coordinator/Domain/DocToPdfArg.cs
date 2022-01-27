namespace Domain
{
    public class DocToPdfArg
    {
        public int CaseId { get; set; }

        public int DocumentId { get; set; }

        public string DocumentUrl { get; set; }

        public string TransactionId { get; set; }
    }
}
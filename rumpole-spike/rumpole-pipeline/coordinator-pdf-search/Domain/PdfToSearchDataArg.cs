namespace Domain
{
    public class PdfToSearchDataArg
    {
        public int CaseId { get; set; }

        public int DocumentId { get; set; }

        public string SasLink { get; set; }

        public string TransactionId { get; set; }
    }
}
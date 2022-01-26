namespace Domain
{
    public class PngToSearchDataArg
    {
        public int CaseId { get; set; }

        public int DocumentId { get; set; }

        public int PageIndex { get; set; }

        public string SasLink { get; set; }
    }
}
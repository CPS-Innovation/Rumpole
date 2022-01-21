using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Domain
{
    public class SearchLine : Line
    {
        public string Id { get; set; }

        public int CaseId { get; set; }

        public int DocumentId { get; set; }

        public int PageIndex { get; set; }

        public int LineIndex { get; set; }
    }
}
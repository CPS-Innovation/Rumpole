using System;

namespace RumpoleGateway.Domain.CoreDataApi
{
    public class Case
    {
        public int Id { get; set; }
        public string UniqueReferenceNumber { get; set; }
        public string AppealType { get; set; }
        public string caseType { get; set; }

    }
}

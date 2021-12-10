using System;

namespace RumpoleGateway.Domain.CoreDataApi
{
    public class CaseInformation
    {
        public int Id { get; set; }
        public Guid UniqueReferenceNumber { get; set; }
        public string AppealType { get; set; }

    }
}

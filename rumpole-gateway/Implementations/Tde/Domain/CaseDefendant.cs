
using System.Collections.Generic;

namespace RumpoleGateway.Implementations.Tde.Domain
{
    public class CaseDefendant
    {
        public int Id { get; set; }
        public int? ListOrder { get; set; }
        public string Type { get; set; }
        public string FirstNames { get; set; }
        public string Surname { get; set; }
        public string Dob { get; set; }
        public string RemandStatus { get; set; }
        public bool Youth { get; set; }
        public CustodyTimeLimit CustodyTimeLimit { get; set; }

        public IEnumerable<Offence> Offences { get; set; }

        public NextHearing NextHearing { get; set; }
    }

    public class CustodyTimeLimit
    {
        public string ExpiryDate { get; set; }
        public int? ExpiryDays { get; set; }
        public string ExpiryIndicator { get; set; }
    }

    public class Offence
    {
        public int Id { get; set; }
        public int? ListOrder { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Active { get; set; }
        public string Description { get; set; }
        public string FromDate { get; set; }
        public string LatestPlea { get; set; }
        public string LatestVerdict { get; set; }
        public string DisposedReason { get; set; }
        public string LastHearingOutcome { get; set; }

        public CustodyTimeLimit CustodyTimeLimit { get; set; }
    }

    public class NextHearing
    {
        public string Date { get; set; }
        public bool Recorded { get; set; }
        public string Type { get; set; }
        public string TypeCode { get; set; }
        public string Venue { get; set; }
        public string VenueCode { get; set; }

    }
}
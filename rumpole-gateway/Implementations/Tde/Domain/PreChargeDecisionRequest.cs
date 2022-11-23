using System.Collections.Generic;

namespace RumpoleGateway.Implementations.Tde.Domain
{
    public class PreChargeDecisionRequest
    {
        public int Id { get; set; }

        public string DecisionRequiredBy { get; set; }

        public string DecisionRequested { get; set; }

        public string Surname { get; set; }

        public string FirstNames { get; set; }

        public string Dob { get; set; }

        public string BailConditions { get; set; }

        public string BailDate { get; set; }

        public string RemandStatus { get; set; }

        public IEnumerable<ProposedCharge> PropsedCharges { get; set; }

    }

    public class ProposedCharge
    {
        public string Charge { get; set; }
        public string Date { get; set; }
    }
}
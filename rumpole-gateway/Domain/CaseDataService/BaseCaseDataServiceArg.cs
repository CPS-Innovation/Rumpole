using System;

namespace RumpoleGateway.Domain.CaseDataService
{
    public abstract class BaseCaseDataServiceArg
    {
        public string OnBehalfOfToken { get; set; }
        public string UpstreamToken { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
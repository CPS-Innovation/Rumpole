using System;
using System.Threading.Tasks;
using RumpoleGateway.Domain.RumpolePipeline;

namespace RumpoleGateway.Clients.RumpolePipeline
{
	public interface IPipelineClient
	{
		Task TriggerCoordinatorAsync(string caseUrn, string caseId, string accessToken, string upstreamToken, bool force, Guid correlationId);
		
        Task<Tracker> GetTrackerAsync(string caseUrn, string caseId, string accessToken, Guid correlationId);
    }
}


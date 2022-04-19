using System.Net.Http;
using System.Threading.Tasks;
using RumpoleGateway.Domain.RumpolePipeline;

namespace RumpoleGateway.Clients.RumpolePipeline
{
	public interface IPipelineClient
	{
		Task<HttpResponseMessage> TriggerCoordinator(string caseId, string accessToken);
		Task<Tracker> GetTracker(string caseId, string accessToken);
	}
}


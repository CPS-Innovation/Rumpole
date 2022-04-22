using System.Net.Http;
using System.Threading.Tasks;
using RumpoleGateway.Domain.RumpolePipeline;

namespace RumpoleGateway.Clients.RumpolePipeline
{
	public interface IPipelineClient
	{
		Task<HttpResponseMessage> TriggerCoordinatorAsync(string caseId, string accessToken);
		Task<Tracker> GetTrackerAsync(string caseId, string accessToken);
	}
}


using Microsoft.AspNetCore.Http;
using RumpoleGateway.Domain.RumpolePipeline;

namespace RumpoleGateway.Factories
{
	public interface ITriggerCoordinatorResponseFactory
	{
		TriggerCoordinatorResponse Create(HttpRequest request);
	}
}


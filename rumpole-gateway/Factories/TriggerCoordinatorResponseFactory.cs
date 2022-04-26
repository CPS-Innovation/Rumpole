using Microsoft.AspNetCore.Http;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Mappers;

namespace RumpoleGateway.Factories
{
	public class TriggerCoordinatorResponseFactory : ITriggerCoordinatorResponseFactory
	{
        private readonly ITrackerUrlMapper _trackerUrlMapper;

        public TriggerCoordinatorResponseFactory(ITrackerUrlMapper trackerUrlMapper)
        {
            _trackerUrlMapper = trackerUrlMapper;
        }

		public TriggerCoordinatorResponse Create(HttpRequest request)
        {
            var url = _trackerUrlMapper.Map(request);
            return new TriggerCoordinatorResponse { TrackerUrl = url };
        }
	}
}


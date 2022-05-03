﻿using System.Threading.Tasks;
using RumpoleGateway.Domain.RumpolePipeline;

namespace RumpoleGateway.Clients.RumpolePipeline
{
	public interface IPipelineClient
	{
		Task TriggerCoordinatorAsync(string caseId, string accessToken);
		Task<Tracker> GetTrackerAsync(string caseId, string accessToken);
	}
}

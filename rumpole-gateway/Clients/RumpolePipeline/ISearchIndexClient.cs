using System.Collections.Generic;
using System.Threading.Tasks;
using RumpoleGateway.Domain.RumpolePipeline;

namespace RumpoleGateway.Clients.RumpolePipeline
{
	public interface ISearchIndexClient
	{
		Task<IList<SearchLine>> Query(int caseId, string searchTerm);
	}
}


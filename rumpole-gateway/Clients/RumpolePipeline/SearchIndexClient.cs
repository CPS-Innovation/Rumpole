using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Search.Documents;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Factories;

namespace RumpoleGateway.Clients.RumpolePipeline
{
	public class SearchIndexClient : ISearchIndexClient
	{
		private readonly SearchClient _searchClient;

        public SearchIndexClient(ISearchClientFactory searchClientFactory)
		{
			_searchClient = searchClientFactory.Create();
        }

		public async Task<IList<SearchLine>> Query(int caseId, string searchTerm)
		{
			var searchOptions = new SearchOptions
			{
				Filter = $"caseId eq {caseId}"
			};
			searchOptions.OrderBy.Add("DocumentId");
			
			var searchResults = await _searchClient.SearchAsync<SearchLine>(searchTerm, searchOptions);

			var searchLines = new List<SearchLine>();
			await foreach (var searchResult in searchResults.Value.GetResultsAsync())
			{
				if (searchResult.Document != null && searchLines.Find(sl => sl.Id == searchResult.Document.Id) == null)
					searchLines.Add(searchResult.Document);
			}

			return searchLines;
		}
	}
}
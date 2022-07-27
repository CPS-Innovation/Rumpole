using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Search.Documents;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Factories;

namespace RumpoleGateway.Clients.RumpolePipeline
{
	public class SearchIndexClient : ISearchIndexClient
	{
		private readonly SearchClient _searchClient;
		private readonly IStreamlinedSearchResultFactory _streamlinedSearchResultFactory;

        public SearchIndexClient(ISearchClientFactory searchClientFactory, IStreamlinedSearchResultFactory streamlinedSearchResultFactory)
		{
			_searchClient = searchClientFactory.Create();
			_streamlinedSearchResultFactory = streamlinedSearchResultFactory;
		}

		public async Task<IList<StreamlinedSearchLine>> Query(int caseId, string searchTerm)
		{
			var searchOptions = new SearchOptions
			{
				Filter = $"caseId eq {caseId}"
			};
			searchOptions.OrderBy.Add("id");
			
			var searchResults = await _searchClient.SearchAsync<SearchLine>(searchTerm, searchOptions);

			var searchLines = new List<SearchLine>();
			await foreach (var searchResult in searchResults.Value.GetResultsAsync())
			{
				//if (searchResult.Document != null && searchLines.Find(sl => sl.Id == searchResult.Document.Id) == null)
					searchLines.Add(searchResult.Document);
			}

            return BuildStreamlinedResults(searchLines, searchTerm);
        }

        public IList<StreamlinedSearchLine> BuildStreamlinedResults(IList<SearchLine> searchResults, string searchTerm)
        {
            var streamlinedResults = new List<StreamlinedSearchLine>();
            if (searchResults.Count == 0)
                return streamlinedResults;

            streamlinedResults.AddRange(searchResults
	            .Select(searchResult => _streamlinedSearchResultFactory.Create(searchResult, searchTerm)));

            return streamlinedResults;
        }
    }
}
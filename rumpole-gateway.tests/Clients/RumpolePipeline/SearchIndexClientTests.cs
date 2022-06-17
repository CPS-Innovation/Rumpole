using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using FluentAssertions;
using Moq;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Factories;
using Xunit;

namespace RumpoleGateway.Tests.Clients.RumpolePipeline
{
	public class SearchIndexClientTests
	{
		private Fixture _fixture;
		private int _caseId;
		private string _searchTerm;

		private Mock<ISearchClientFactory> _mockSearchClientFactory;
		private Mock<SearchClient> _mockSearchClient;
		private Mock<Response<SearchResults<SearchLine>>> _mockResponse;
		private Mock<SearchResults<SearchLine>> _mockSearchResults;

		private ISearchIndexClient SearchIndexClient;

		public SearchIndexClientTests()
		{
			_fixture = new Fixture();
			_caseId = _fixture.Create<int>();
			_searchTerm = _fixture.Create<string>();

			_mockSearchClientFactory = new Mock<ISearchClientFactory>();
			_mockSearchClient = new Mock<SearchClient>();
			_mockResponse = new Mock<Response<SearchResults<SearchLine>>>();
			_mockSearchResults = new Mock<SearchResults<SearchLine>>();

			_mockSearchClientFactory.Setup(factory => factory.Create()).Returns(_mockSearchClient.Object);
			_mockSearchClient.Setup(client => client.SearchAsync<SearchLine>(_searchTerm, It.Is<SearchOptions>(o => o.Filter == $"caseId eq {_caseId}"), It.IsAny<CancellationToken>()))
				.ReturnsAsync(_mockResponse.Object);
			_mockResponse.Setup(response => response.Value).Returns(_mockSearchResults.Object);

			SearchIndexClient = new SearchIndexClient(_mockSearchClientFactory.Object);
		}

        [Fact]
		public async Task Query_ReturnsSearchLines()
        {
			var results = SearchIndexClient.Query(_caseId, _searchTerm);

			results.Should().NotBeNull();
        }
	}
}


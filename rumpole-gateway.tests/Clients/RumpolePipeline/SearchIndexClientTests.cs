using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Factories;
using Xunit;

namespace RumpoleGateway.Tests.Clients.RumpolePipeline
{
	public class SearchIndexClientTests
	{
		private readonly Fixture _fixture;
		private readonly int _caseId;
		private readonly string _searchTerm;

		private readonly Mock<SearchClient> _mockSearchClient;
		private readonly Mock<Response<SearchResults<SearchLine>>> _mockResponse;

		private readonly ISearchIndexClient _searchIndexClient;

		public SearchIndexClientTests()
		{
			_fixture = new Fixture();
			_caseId = _fixture.Create<int>();
			_searchTerm = _fixture.Create<string>();

			var mockSearchClientFactory = new Mock<ISearchClientFactory>();
			_mockSearchClient = new Mock<SearchClient>();
			_mockResponse = new Mock<Response<SearchResults<SearchLine>>>();
			var mockSearchResults = new Mock<SearchResults<SearchLine>>();

			mockSearchClientFactory.Setup(factory => factory.Create()).Returns(_mockSearchClient.Object);
			_mockSearchClient.Setup(client => client.SearchAsync<SearchLine>(_searchTerm, It.Is<SearchOptions>(o => o.Filter == $"caseId eq {_caseId}"), It.IsAny<CancellationToken>()))
				.ReturnsAsync(_mockResponse.Object);
			_mockResponse.Setup(response => response.Value).Returns(mockSearchResults.Object);

			_searchIndexClient = new SearchIndexClient(mockSearchClientFactory.Object);
		}

        [Fact]
		public async Task Query_ReturnsSearchLines()
        {
			var results = await _searchIndexClient.Query(_caseId, _searchTerm);

			results.Should().NotBeNull();
        }

		[Fact]
		public async Task Query_WhenResultsContainDuplicates_ShouldReturnNoDuplicates()
		{
			var responseMock = new Mock<Response>();
			var fakeSearchLines = _fixture.CreateMany<SearchLine>(3).ToList();
			var duplicateRecord = fakeSearchLines[0];
			var duplicateRecordId = duplicateRecord.Id;
			fakeSearchLines.Add(duplicateRecord);

			_mockSearchClient.Setup(client => client.SearchAsync<SearchLine>(_searchTerm, 
					It.Is<SearchOptions>(o => o.Filter == $"caseId eq {_caseId}"), It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(
						Response.FromValue(
							SearchModelFactory.SearchResults<SearchLine>(new[] {
								SearchModelFactory.SearchResult(fakeSearchLines[0], 0.9, null),
								SearchModelFactory.SearchResult(fakeSearchLines[1], 0.8, null),
								SearchModelFactory.SearchResult(fakeSearchLines[2], 0.8, null),
								SearchModelFactory.SearchResult(fakeSearchLines[3], 0.9, null)
							}, 100, null, null, responseMock.Object), responseMock.Object)));
			
			var results = await _searchIndexClient.Query(_caseId, _searchTerm);

			using (new AssertionScope())
			{
				results.Count.Should().Be(3);
				results.Count(s => s.Id == duplicateRecordId).Should().Be(1);
			}
		}
	}
}


using AutoFixture;
using Azure.Search.Documents;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Factories;
using Xunit;

namespace RumpoleGateway.Tests.Factories
{
	public class SearchClientFactoryTests
	{
		private Fixture _fixture;
		private Domain.RumpolePipeline.SearchClientOptions _searchIndexOptions;

		private Mock<IOptions<Domain.RumpolePipeline.SearchClientOptions>> _mockSearchIndexOptions;

		private ISearchClientFactory SearchClientFactory;

		public SearchClientFactoryTests()
		{
			_fixture = new Fixture();
			_searchIndexOptions = _fixture.Build<SearchClientOptions>()
									.With(o => o.EndpointUrl, "https://www.google.co.uk")
									.Create();

            _mockSearchIndexOptions = new Mock<IOptions<Domain.RumpolePipeline.SearchClientOptions>>();

			_mockSearchIndexOptions.Setup(options => options.Value).Returns(_searchIndexOptions);

			SearchClientFactory = new SearchClientFactory(_mockSearchIndexOptions.Object);
		}

        [Fact]
		public void Create_ReturnsSearchClient()
        {
			var searchClient = SearchClientFactory.Create();

			searchClient.Should().BeOfType<SearchClient>();
        }

		[Fact]
		public void Create_SetsExpectedEndpoint()
		{
			var searchClient = SearchClientFactory.Create();

			searchClient.Endpoint.Should().Be(_searchIndexOptions.EndpointUrl);
		}

		[Fact]
		public void Create_SetsExpectedIndexName()
		{
			var searchClient = SearchClientFactory.Create();

			searchClient.IndexName.Should().Be(_searchIndexOptions.IndexName);
		}
	}
}


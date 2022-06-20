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
		private Domain.RumpolePipeline.SearchClientOptions _searchClientOptions;

		private Mock<IOptions<Domain.RumpolePipeline.SearchClientOptions>> _mockSearchClientOptions;

		private ISearchClientFactory SearchClientFactory;

		public SearchClientFactoryTests()
		{
			_fixture = new Fixture();
			_searchClientOptions = _fixture.Build<Domain.RumpolePipeline.SearchClientOptions>()
									.With(o => o.EndpointUrl, "https://www.google.co.uk")
									.Create();

            _mockSearchClientOptions = new Mock<IOptions<Domain.RumpolePipeline.SearchClientOptions>>();

			_mockSearchClientOptions.Setup(options => options.Value).Returns(_searchClientOptions);

			SearchClientFactory = new SearchClientFactory(_mockSearchClientOptions.Object);
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

			searchClient.Endpoint.Should().Be(_searchClientOptions.EndpointUrl);
		}

		[Fact]
		public void Create_SetsExpectedIndexName()
		{
			var searchClient = SearchClientFactory.Create();

			searchClient.IndexName.Should().Be(_searchClientOptions.IndexName);
		}
	}
}


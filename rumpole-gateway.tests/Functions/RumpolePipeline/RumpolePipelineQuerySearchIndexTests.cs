using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Azure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Functions.RumpolePipeline;
using Xunit;

namespace RumpoleGateway.Tests.Functions.RumpolePipeline
{
	public class RumpolePipelineQuerySearchIndexTests : SharedMethods.SharedMethods
	{
		private Fixture _fixture;
		private int _caseIdInt;
		private string _caseId;
		private string _searchTerm;
		private IList<SearchLine> _searchResults;

		private Mock<ILogger<RumpolePipelineQuerySearchIndex>> _mockLogger;
		private Mock<ISearchIndexClient> _searchIndexClient;

		private RumpolePipelineQuerySearchIndex RumpolePipelineQuerySearchIndex;

		public RumpolePipelineQuerySearchIndexTests()
		{
			_fixture = new Fixture();
			_caseIdInt = _fixture.Create<int>();
			_caseId = _caseIdInt.ToString();
			_searchTerm = _fixture.Create<string>();
			_searchResults = _fixture.Create<IList<SearchLine>>();

			_mockLogger = new Mock<ILogger<RumpolePipelineQuerySearchIndex>>();
			_searchIndexClient = new Mock<ISearchIndexClient>();

			_searchIndexClient.Setup(client => client.Query(_caseIdInt, _searchTerm))
				.ReturnsAsync(_searchResults);

			RumpolePipelineQuerySearchIndex = new RumpolePipelineQuerySearchIndex(_mockLogger.Object, _searchIndexClient.Object);
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
        {
			var response = await RumpolePipelineQuerySearchIndex.Run(CreateHttpRequestWithoutToken(), _caseId, _searchTerm);

			response.Should().BeOfType<UnauthorizedObjectResult>();
        }

		[Fact]
		public async Task Run_ReturnsBadRequestWhenCaseIdIsNotAnInteger()
		{
			var response = await RumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), "Not an integer", _searchTerm);

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Theory]
        [InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public async Task Run_ReturnsBadRequestWhenSearchTermIsInvalid(string searchTerm)
		{
			var response = await RumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), _caseId, searchTerm);

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await RumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), _caseId, _searchTerm);

			response.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsSearchResults()
		{
			var response = await RumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), _caseId, _searchTerm) as OkObjectResult;

			response.Value.Should().Be(_searchResults);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenRequestFailedExceptionOccurs()
        {
			_searchIndexClient.Setup(client => client.Query(_caseIdInt, _searchTerm))
				.ThrowsAsync(new RequestFailedException("Test"));

			var response = await RumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), _caseId, _searchTerm) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}


		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_searchIndexClient.Setup(client => client.Query(_caseIdInt, _searchTerm))
				.ThrowsAsync(new RequestFailedException("Test"));

			var response = await RumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), _caseId, _searchTerm) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}
	}
}


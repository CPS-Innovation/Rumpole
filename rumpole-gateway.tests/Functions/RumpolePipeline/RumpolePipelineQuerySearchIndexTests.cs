using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Azure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Functions.RumpolePipeline;
using Xunit;

namespace RumpoleGateway.Tests.Functions.RumpolePipeline
{
	public class RumpolePipelineQuerySearchIndexTests : SharedMethods.SharedMethods
	{
        private readonly int _caseIdInt;
		private readonly string _caseId;
		private readonly string _searchTerm;
		private readonly IList<StreamlinedSearchLine> _searchResults;

        private readonly Mock<ISearchIndexClient> _searchIndexClient;

		private readonly RumpolePipelineQuerySearchIndex _rumpolePipelineQuerySearchIndex;

		public RumpolePipelineQuerySearchIndexTests()
		{
            var fixture = new Fixture();
			_caseIdInt = fixture.Create<int>();
			_caseId = _caseIdInt.ToString();
			_searchTerm = fixture.Create<string>();
			_searchResults = fixture.Create<IList<StreamlinedSearchLine>>();

			var mockLogger = new Mock<ILogger<RumpolePipelineQuerySearchIndex>>();
			_searchIndexClient = new Mock<ISearchIndexClient>();

			_searchIndexClient.Setup(client => client.Query(_caseIdInt, _searchTerm))
				.ReturnsAsync(_searchResults);

            var mockTokenValidator = new Mock<ITokenValidator>();

            mockTokenValidator.Setup(x => x.ValidateTokenAsync(It.IsAny<StringValues>())).ReturnsAsync(true);

            _rumpolePipelineQuerySearchIndex = new RumpolePipelineQuerySearchIndex(mockLogger.Object, _searchIndexClient.Object, mockTokenValidator.Object);
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
        {
			var response = await _rumpolePipelineQuerySearchIndex.Run(CreateHttpRequestWithoutToken(), _caseId, _searchTerm);

			response.Should().BeOfType<UnauthorizedObjectResult>();
        }

		[Fact]
		public async Task Run_ReturnsBadRequestWhenCaseIdIsNotAnInteger()
		{
			var response = await _rumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), "Not an integer", _searchTerm);

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Theory]
        [InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public async Task Run_ReturnsBadRequestWhenSearchTermIsInvalid(string searchTerm)
		{
			var response = await _rumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), _caseId, searchTerm);

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await _rumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), _caseId, _searchTerm);

			response.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsSearchResults()
		{
			var response = await _rumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), _caseId, _searchTerm) as OkObjectResult;

			response?.Value.Should().Be(_searchResults);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenRequestFailedExceptionOccurs()
        {
			_searchIndexClient.Setup(client => client.Query(_caseIdInt, _searchTerm))
				.ThrowsAsync(new RequestFailedException("Test"));

			var response = await _rumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), _caseId, _searchTerm) as StatusCodeResult;

			response?.StatusCode.Should().Be(500);
		}


		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_searchIndexClient.Setup(client => client.Query(_caseIdInt, _searchTerm))
				.ThrowsAsync(new RequestFailedException("Test"));

			var response = await _rumpolePipelineQuerySearchIndex.Run(CreateHttpRequest(), _caseId, _searchTerm) as StatusCodeResult;

			response?.StatusCode.Should().Be(500);
		}
	}
}


using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using Azure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Functions.RumpolePipeline;
using Xunit;

namespace RumpoleGateway.Tests.Functions.RumpolePipeline
{
	public class RumpolePipelineGetPdfTests : SharedMethods.SharedMethods
	{
		private Fixture _fixture;
		private string _blobName;
		private Stream _blobStream;

		private Mock<IBlobStorageClient> _mockBlobStorageClient;
		private Mock<ILogger<RumpolePipelineGetPdf>> _mockLogger;

		private RumpolePipelineGetPdf RumpolePipelineGetPdf;

		public RumpolePipelineGetPdfTests()
		{
			_fixture = new Fixture();
			_blobName = _fixture.Create<string>();
			_blobStream = new MemoryStream();

			_mockBlobStorageClient = new Mock<IBlobStorageClient>();
            _mockLogger = new Mock<ILogger<RumpolePipelineGetPdf>>();
            var mockTokenValidator = new Mock<ITokenValidator>();

            mockTokenValidator.Setup(x => x.ValidateTokenAsync(It.IsAny<StringValues>())).ReturnsAsync(true);

            _mockBlobStorageClient.Setup(client => client.GetDocumentAsync(_blobName)).ReturnsAsync(_blobStream);

			RumpolePipelineGetPdf = new RumpolePipelineGetPdf(_mockBlobStorageClient.Object, _mockLogger.Object, mockTokenValidator.Object);
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
        {
			var response = await RumpolePipelineGetPdf.Run(CreateHttpRequestWithoutToken(), _blobName);

			response.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public async Task Run_ReturnsBadRequestWhenBlobNameIsInvalid(string blobName)
		{
			var response = await RumpolePipelineGetPdf.Run(CreateHttpRequest(), blobName);

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsNotFoundWhenBlobStorageClientReturnsNull()
		{
			_mockBlobStorageClient.Setup(client => client.GetDocumentAsync(_blobName)).ReturnsAsync(default(Stream));

			var response = await RumpolePipelineGetPdf.Run(CreateHttpRequest(), _blobName);

			response.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await RumpolePipelineGetPdf.Run(CreateHttpRequest(), _blobName);

			response.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsBlobStream()
		{
			var response = await RumpolePipelineGetPdf.Run(CreateHttpRequest(), _blobName) as OkObjectResult;

			response.Value.Should().Be(_blobStream);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenRequestFailedExceptionOccurs()
		{
			_mockBlobStorageClient.Setup(client => client.GetDocumentAsync(_blobName))
				.ThrowsAsync(new RequestFailedException(500, "Test request failed exception"));

			var response = await RumpolePipelineGetPdf.Run(CreateHttpRequest(), _blobName) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_mockBlobStorageClient.Setup(client => client.GetDocumentAsync(_blobName))
				.ThrowsAsync(new Exception());

			var response = await RumpolePipelineGetPdf.Run(CreateHttpRequest(), _blobName) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}
	}
}


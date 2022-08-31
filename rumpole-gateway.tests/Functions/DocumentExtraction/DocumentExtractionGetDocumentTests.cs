using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using RumpoleGateway.Clients.DocumentExtraction;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Functions.DocumentExtraction;
using Xunit;

namespace RumpoleGateway.Tests.Functions.DocumentExtraction
{
    public class DocumentExtractionGetDocumentTests : SharedMethods.SharedMethods
	{
        private readonly string _documentId;
		private readonly string _fileName;
		private readonly Stream _stream;

        private readonly Mock<IDocumentExtractionClient> _mockDocumentExtractionClient;

        private readonly DocumentExtractionGetDocument _documentExtractionGetDocument;

		public DocumentExtractionGetDocumentTests()
		{
            var fixture = new Fixture();
			_documentId = fixture.Create<string>();
			_fileName = fixture.Create<string>();
			_stream = new MemoryStream();

			_mockDocumentExtractionClient = new Mock<IDocumentExtractionClient>();
			var mockLogger = new Mock<ILogger<DocumentExtractionGetCaseDocuments>>();
            var mockTokenValidator = new Mock<ITokenValidator>();

            mockTokenValidator.Setup(x => x.ValidateTokenAsync(It.IsAny<StringValues>())).ReturnsAsync(true);

            _mockDocumentExtractionClient.Setup(client => client.GetDocumentAsync(_documentId, _fileName, It.IsAny<string>())) //TODO replace It.IsAny
				.ReturnsAsync(_stream);

			_documentExtractionGetDocument = new DocumentExtractionGetDocument(_mockDocumentExtractionClient.Object, mockLogger.Object, mockTokenValidator.Object);
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
		{
			var response = await _documentExtractionGetDocument.Run(CreateHttpRequestWithoutToken(), _documentId, _fileName);

			response.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public async Task Run_ReturnsBadRequestWhenDocumentIdIsMissing(string documentId)
		{
			var response = await _documentExtractionGetDocument.Run(CreateHttpRequest(), documentId, _fileName);

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public async Task Run_ReturnsBadRequestWhenFileNameIsMissing(string fileName)
		{
			var response = await _documentExtractionGetDocument.Run(CreateHttpRequest(), _documentId, fileName);

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await _documentExtractionGetDocument.Run(CreateHttpRequest(), _documentId, _fileName);

			response.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsStream()
        {
            var response = await _documentExtractionGetDocument.Run(CreateHttpRequest(), _documentId, _fileName) as OkObjectResult;

            response?.Value.Should().Be(_stream);
        }

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_mockDocumentExtractionClient.Setup(client => client.GetDocumentAsync(_documentId, _fileName, It.IsAny<string>())) //TODO replace It.IsAny
				.ThrowsAsync(new Exception());

			var response = await _documentExtractionGetDocument.Run(CreateHttpRequest(), _documentId, _fileName) as StatusCodeResult;

			response?.StatusCode.Should().Be(500);
		}
	}
}


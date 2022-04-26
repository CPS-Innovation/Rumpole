using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RumpoleGateway.Clients.DocumentExtraction;
using RumpoleGateway.Functions.CoreDataApi;
using Xunit;

namespace RumpoleGateway.Tests.Functions.DocumentExtraction
{
	public class DocumentExtractionGetDocumentTests : SharedMethods.SharedMethods
	{
		private Fixture _fixture;
		private string _documentId;
		private string _fileName;
		private Stream _stream;

        private Mock<IDocumentExtractionClient> _mockDocumentExtractionClient;
		private Mock<ILogger<DocumentExtractionGetCaseDocuments>> _mockLogger;

		private DocumentExtractionGetDocument DocumentExtractionGetDocument;

		public DocumentExtractionGetDocumentTests()
		{
			_fixture = new Fixture();
			_documentId = _fixture.Create<string>();
			_fileName = _fixture.Create<string>();
			_stream = new MemoryStream();

			_mockDocumentExtractionClient = new Mock<IDocumentExtractionClient>();
			_mockLogger = new Mock<ILogger<DocumentExtractionGetCaseDocuments>>();

			_mockDocumentExtractionClient.Setup(client => client.GetDocumentAsync(_documentId, _fileName, It.IsAny<string>())) //TODO replace It.IsAny
				.ReturnsAsync(_stream);

			DocumentExtractionGetDocument = new DocumentExtractionGetDocument(_mockDocumentExtractionClient.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
		{
			var response = await DocumentExtractionGetDocument.Run(CreateHttpRequestWithoutToken(), _documentId, _fileName);

			response.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public async Task Run_ReturnsBadRequestWhenDocumentIdIsMissing(string documentId)
		{
			var response = await DocumentExtractionGetDocument.Run(CreateHttpRequest(), documentId, _fileName);

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public async Task Run_ReturnsBadRequestWhenFileNameIsMissing(string fileName)
		{
			var response = await DocumentExtractionGetDocument.Run(CreateHttpRequest(), _documentId, fileName);

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await DocumentExtractionGetDocument.Run(CreateHttpRequest(), _documentId, _fileName);

			response.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsStream()
		{
			var response = await DocumentExtractionGetDocument.Run(CreateHttpRequest(), _documentId, _fileName) as OkObjectResult;

			response.Value.Should().Be(_stream);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_mockDocumentExtractionClient.Setup(client => client.GetDocumentAsync(_documentId, _fileName, It.IsAny<string>())) //TODO replace It.IsAny
				.ThrowsAsync(new Exception());

			var response = await DocumentExtractionGetDocument.Run(CreateHttpRequest(), _documentId, _fileName) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}
	}
}


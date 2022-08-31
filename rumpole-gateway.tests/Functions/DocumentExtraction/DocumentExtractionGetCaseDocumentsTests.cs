using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using RumpoleGateway.Clients.DocumentExtraction;
using RumpoleGateway.Domain.DocumentExtraction;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Functions.DocumentExtraction;
using Xunit;

namespace RumpoleGateway.Tests.Functions.DocumentExtraction
{
    public class DocumentExtractionGetCaseDocumentsTests : SharedMethods.SharedMethods
	{
        private readonly string _caseId;
		private readonly Case _case;

        private readonly Mock<IDocumentExtractionClient> _mockDocumentExtractionClient;

        private readonly DocumentExtractionGetCaseDocuments _documentExtractionGetCaseDocuments;

		public DocumentExtractionGetCaseDocumentsTests()
		{
            var fixture = new Fixture();
			_caseId = fixture.Create<int>().ToString();
			_case = fixture.Create<Case>();

			_mockDocumentExtractionClient = new Mock<IDocumentExtractionClient>();
			var mockLogger = new Mock<ILogger<DocumentExtractionGetCaseDocuments>>();
            var mockTokenValidator = new Mock<ITokenValidator>();

            mockTokenValidator.Setup(x => x.ValidateTokenAsync(It.IsAny<StringValues>())).ReturnsAsync(true);
			_mockDocumentExtractionClient.Setup(client => client.GetCaseDocumentsAsync(_caseId, It.IsAny<string>())) //TODO replace It.IsAny
				.ReturnsAsync(_case);

			_documentExtractionGetCaseDocuments = new DocumentExtractionGetCaseDocuments(_mockDocumentExtractionClient.Object, mockLogger.Object, mockTokenValidator.Object);
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
		{
			var response = await _documentExtractionGetCaseDocuments.Run(CreateHttpRequestWithoutToken(), _caseId);

			response.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsBadRequestWhenCaseIdIsNotAnInteger()
		{
			var response = await _documentExtractionGetCaseDocuments.Run(CreateHttpRequest(), "Not an integer");

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsNotFoundWhenPipelineClientReturnsNull()
		{
			_mockDocumentExtractionClient.Setup(client => client.GetCaseDocumentsAsync(_caseId, It.IsAny<string>())) //TODO replace It.IsAny
				.ReturnsAsync(default(Case));

			var response = await _documentExtractionGetCaseDocuments.Run(CreateHttpRequest(), _caseId);

			response.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await _documentExtractionGetCaseDocuments.Run(CreateHttpRequest(), _caseId);

			response.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsCase()
        {
            var response = await _documentExtractionGetCaseDocuments.Run(CreateHttpRequest(), _caseId) as OkObjectResult;

            response?.Value.Should().Be(_case);
        }

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_mockDocumentExtractionClient.Setup(client => client.GetCaseDocumentsAsync(_caseId, It.IsAny<string>())) //TODO replace It.IsAny
				.ThrowsAsync(new Exception());

			var response = await _documentExtractionGetCaseDocuments.Run(CreateHttpRequest(), _caseId) as StatusCodeResult;

            response?.StatusCode.Should().Be(500);
        }
	}
}


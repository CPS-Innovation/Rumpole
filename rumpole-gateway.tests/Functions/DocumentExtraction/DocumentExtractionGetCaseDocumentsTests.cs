using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RumpoleGateway.Clients.DocumentExtraction;
using RumpoleGateway.Domain.DocumentExtraction;
using RumpoleGateway.Functions.CoreDataApi;
using Xunit;

namespace RumpoleGateway.Tests.Functions.DocumentExtraction
{
	public class DocumentExtractionGetCaseDocumentsTests : SharedMethods.SharedMethods
	{
		private Fixture _fixture;
		private string _caseId;
		private Case _case;


        private Mock<IDocumentExtractionClient> _mockDocumentExtractionClient;
		private Mock<ILogger<DocumentExtractionGetCaseDocuments>> _mockLogger;

		private DocumentExtractionGetCaseDocuments DocumentExtractionGetCaseDocuments;

		public DocumentExtractionGetCaseDocumentsTests()
		{
			_fixture = new Fixture();
			_caseId = _fixture.Create<int>().ToString();
			_case = _fixture.Create<Case>();

			_mockDocumentExtractionClient = new Mock<IDocumentExtractionClient>();
			_mockLogger = new Mock<ILogger<DocumentExtractionGetCaseDocuments>>();

			_mockDocumentExtractionClient.Setup(client => client.GetCaseDocumentsAsync(_caseId, It.IsAny<string>())) //TODO replace It.IsAny
				.ReturnsAsync(_case);

			DocumentExtractionGetCaseDocuments = new DocumentExtractionGetCaseDocuments(_mockDocumentExtractionClient.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
		{
			var response = await DocumentExtractionGetCaseDocuments.Run(CreateHttpRequestWithoutToken(), _caseId);

			response.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsBadRequestWhenCaseIdIsNotAnInteger()
		{
			var response = await DocumentExtractionGetCaseDocuments.Run(CreateHttpRequest(), "Not an integer");

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsNotFoundWhenPipelineClientReturnsNull()
		{
			_mockDocumentExtractionClient.Setup(client => client.GetCaseDocumentsAsync(_caseId, It.IsAny<string>())) //TODO replace It.IsAny
				.ReturnsAsync(default(Case));

			var response = await DocumentExtractionGetCaseDocuments.Run(CreateHttpRequest(), _caseId);

			response.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await DocumentExtractionGetCaseDocuments.Run(CreateHttpRequest(), _caseId);

			response.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsCase()
		{
			var response = await DocumentExtractionGetCaseDocuments.Run(CreateHttpRequest(), _caseId) as OkObjectResult;

			response.Value.Should().Be(_case);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_mockDocumentExtractionClient.Setup(client => client.GetCaseDocumentsAsync(_caseId, It.IsAny<string>())) //TODO replace It.IsAny
				.ThrowsAsync(new Exception());

			var response = await DocumentExtractionGetCaseDocuments.Run(CreateHttpRequest(), _caseId) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}
	}
}


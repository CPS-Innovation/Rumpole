using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using RumpoleGateway.Clients.DocumentRedaction;
using RumpoleGateway.Domain.DocumentRedaction;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Functions.DocumentRedaction;
using Xunit;

namespace RumpoleGateway.Tests.Functions.DocumentRedaction
{
    public class DocumentRedactionCheckOutTests : SharedMethods.SharedMethods
    {
        private readonly string _caseId;
        private readonly string _documentId;

        private readonly Mock<IDocumentRedactionClient> _mockDocumentRedactionClient;

        private readonly DocumentRedactionCheckOutDocument _documentRedactionCheckOutFunction;

        public DocumentRedactionCheckOutTests()
        {
            var fixture = new Fixture();
            _caseId = fixture.Create<int>().ToString();
            _documentId = fixture.Create<string>();

            _mockDocumentRedactionClient = new Mock<IDocumentRedactionClient>();
            var mockLogger = new Mock<ILogger<DocumentRedactionCheckOutDocument>>();
            var mockTokenValidator = new Mock<ITokenValidator>();

            mockTokenValidator.Setup(x => x.ValidateTokenAsync(It.IsAny<StringValues>())).ReturnsAsync(true);

            _mockDocumentRedactionClient
                .Setup(s => s.CheckOutDocumentAsync(_caseId, _documentId, It.IsAny<string>()))
                .ReturnsAsync(DocumentRedactionStatus.CheckedOut);

            _documentRedactionCheckOutFunction = new DocumentRedactionCheckOutDocument(mockLogger.Object, _mockDocumentRedactionClient.Object, mockTokenValidator.Object);
        }

        [Fact]
        public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
        {
            var response = await _documentRedactionCheckOutFunction.Run(CreateHttpRequestWithoutToken(), _caseId, _documentId);

            response.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Theory]
        [InlineData("x", "12345")]
        [InlineData("", "12345")]
        [InlineData("5.54322", "12345")]
        public async Task Run_ReturnsBadRequestWhenCaseIdIsNotANumber(string caseId, string documentId)
        {
            var response = await _documentRedactionCheckOutFunction.Run(CreateHttpRequest(), caseId, documentId);

            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData("1", null)]
        [InlineData("1", "")]
        [InlineData("1", " ")]
        public async Task Run_ReturnsBadRequestWhenDocumentIdIsMissing(string caseId, string documentId)
        {
            var response = await _documentRedactionCheckOutFunction.Run(CreateHttpRequest(), caseId, documentId);

            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Run_ReturnsOk()
        {
            var response = await _documentRedactionCheckOutFunction.Run(CreateHttpRequest(), _caseId, _documentId);

            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
        {
            _mockDocumentRedactionClient.Setup(client => client.CheckOutDocumentAsync(_caseId, _documentId, It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var response = await _documentRedactionCheckOutFunction.Run(CreateHttpRequest(), _caseId, _documentId) as StatusCodeResult;

            response?.StatusCode.Should().Be(500);
        }
    }
}

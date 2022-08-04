﻿using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RumpoleGateway.Clients.DocumentRedaction;
using RumpoleGateway.Domain.DocumentRedaction;
using RumpoleGateway.Functions.DocumentRedaction;
using Xunit;

namespace RumpoleGateway.Tests.Functions.DocumentRedaction
{
    public class DocumentRedactionCheckInTests : SharedMethods.SharedMethods
    {
        private readonly string _caseId;
        private readonly string _documentId;

        private readonly Mock<IDocumentRedactionClient> _mockDocumentRedactionClient;

        private readonly DocumentRedactionCheckInDocument _documentRedactionCheckInDocument;

        public DocumentRedactionCheckInTests()
        {
            var fixture = new Fixture();
            _caseId = fixture.Create<int>().ToString();
            _documentId = fixture.Create<string>();

            _mockDocumentRedactionClient = new Mock<IDocumentRedactionClient>();
            var mockLogger = new Mock<ILogger<DocumentRedactionCheckInDocument>>();

            _mockDocumentRedactionClient
                .Setup(s => s.CheckInDocument(_caseId, _documentId, It.IsAny<string>()))
                .ReturnsAsync(DocumentRedactionStatus.CheckedIn);

            _documentRedactionCheckInDocument = new DocumentRedactionCheckInDocument(mockLogger.Object, _mockDocumentRedactionClient.Object);
        }

        [Fact]
        public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
        {
            var response = await _documentRedactionCheckInDocument.Run(CreateHttpRequestWithoutToken(), _caseId, _documentId);

            response.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Theory]
        [InlineData("x", "12345")]
        [InlineData("", "12345")]
        [InlineData("5.54322", "12345")]
        public async Task Run_ReturnsBadRequestWhenCaseIdIsNotANumber(string caseId, string documentId)
        {
            var response = await _documentRedactionCheckInDocument.Run(CreateHttpRequest(), caseId, documentId);

            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData("1", null)]
        [InlineData("1", "")]
        [InlineData("1", " ")]
        public async Task Run_ReturnsBadRequestWhenDocumentIdIsMissing(string caseId, string documentId)
        {
            var response = await _documentRedactionCheckInDocument.Run(CreateHttpRequest(), caseId, documentId);

            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Run_ReturnsOk()
        {
            var response = await _documentRedactionCheckInDocument.Run(CreateHttpRequest(), _caseId, _documentId);

            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
        {
            _mockDocumentRedactionClient.Setup(client => client.CheckOutDocument(_caseId, _documentId, It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var response = await _documentRedactionCheckInDocument.Run(CreateHttpRequest(), _caseId, _documentId) as StatusCodeResult;

            response?.StatusCode.Should().Be(500);
        }
    }
}

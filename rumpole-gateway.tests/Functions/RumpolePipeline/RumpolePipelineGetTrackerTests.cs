using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;
using Moq;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Functions.RumpolePipeline;
using Xunit;

namespace RumpoleGateway.Tests.Functions.RumpolePipeline
{
	public class RumpolePipelineGetTrackerTests : SharedMethods.SharedMethods
	{
        private readonly string _caseId;
		private readonly string _onBehalfOfAccessToken;
		private readonly Guid _correlationId;
		private readonly Tracker _tracker;

        private readonly Mock<IOnBehalfOfTokenClient> _mockOnBehalfOfTokenClient;
		private readonly Mock<IPipelineClient> _mockPipelineClient;

        private readonly RumpolePipelineGetTracker _rumpolePipelineGetTracker;

		public RumpolePipelineGetTrackerTests()
		{
			var fixture = new Fixture();
			_caseId = fixture.Create<int>().ToString();
			_onBehalfOfAccessToken = fixture.Create<string>();
			var rumpolePipelineCoordinatorScope = fixture.Create<string>();
			_tracker = fixture.Create<Tracker>();
			_correlationId = fixture.Create<Guid>();

			var mockLogger = new Mock<ILogger<RumpolePipelineGetTracker>>();
			_mockOnBehalfOfTokenClient = new Mock<IOnBehalfOfTokenClient>();
			_mockPipelineClient = new Mock<IPipelineClient>();
			var mockConfiguration = new Mock<IConfiguration>();
            var mockTokenValidator = new Mock<IAuthorizationValidator>();

            mockTokenValidator.Setup(x => x.ValidateTokenAsync(It.IsAny<StringValues>(), It.IsAny<Guid>())).ReturnsAsync(true);
            _mockOnBehalfOfTokenClient.Setup(client => client.GetAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
				.ReturnsAsync(_onBehalfOfAccessToken);
			_mockPipelineClient.Setup(client => client.GetTrackerAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
				.ReturnsAsync(_tracker);
			mockConfiguration.Setup(config => config["RumpolePipelineCoordinatorScope"]).Returns(rumpolePipelineCoordinatorScope);

			_rumpolePipelineGetTracker = new RumpolePipelineGetTracker(mockLogger.Object, _mockOnBehalfOfTokenClient.Object, _mockPipelineClient.Object, mockConfiguration.Object, mockTokenValidator.Object);
		}
		
		[Fact]
		public async Task Run_ReturnsBadRequestWhenAccessCorrelationIdIsMissing()
		{
			var response = await _rumpolePipelineGetTracker.Run(CreateHttpRequestWithoutCorrelationId(), _caseId);

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
        {
			var response = await _rumpolePipelineGetTracker.Run(CreateHttpRequestWithoutToken(), _caseId);

			response.Should().BeOfType<UnauthorizedObjectResult>();
        }

		[Fact]
		public async Task Run_ReturnsBadRequestWhenCaseIdIsNotAnInteger()
		{
			var response = await _rumpolePipelineGetTracker.Run(CreateHttpRequest(), "Not an integer");

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsNotFoundWhenPipelineClientReturnsNull()
		{
			_mockPipelineClient.Setup(client => client.GetTrackerAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
				.ReturnsAsync(default(Tracker));

			var response = await _rumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId);

			response.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await _rumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId);

			response.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsTracker()
		{
			var response = await _rumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId) as OkObjectResult;

			response?.Value.Should().Be(_tracker);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenMsalExceptionOccurs()
        {
			_mockOnBehalfOfTokenClient.Setup(client => client.GetAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
				.ThrowsAsync(new MsalException());

			var response = await _rumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId) as ObjectResult;

			response?.StatusCode.Should().Be(500);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenHttpExceptionOccurs()
		{
			_mockPipelineClient.Setup(client => client.GetTrackerAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
				.ThrowsAsync(new HttpRequestException());

			var response = await _rumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId) as ObjectResult;

			response?.StatusCode.Should().Be(500);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_mockPipelineClient.Setup(client => client.GetTrackerAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
				.ThrowsAsync(new Exception());

			var response = await _rumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId) as ObjectResult;

			response?.StatusCode.Should().Be(500);
		}
	}
}


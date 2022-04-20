using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Moq;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Functions.RumpolePipeline;
using Xunit;

namespace RumpoleGateway.Tests.Functions.RumpolePipeline
{
	public class RumpolePipelineGetTrackerTests : SharedMethods.SharedMethods
	{
		private Fixture _fixture;
		private string _caseId;
		private string _onBehalfOfAccessToken;
		private string _rumpolePipelineCoordinatorScope;
		private Tracker _tracker;

		private Mock<ILogger<RumpolePipelineGetTracker>> _mockLogger;
		private Mock<IOnBehalfOfTokenClient> _mockOnBehalfOfTokenClient;
		private Mock<IPipelineClient> _mockPipelineClient;
		private Mock<IConfiguration> _mockConfiguration;

		private RumpolePipelineGetTracker RumpolePipelineGetTracker;

		public RumpolePipelineGetTrackerTests()
		{
			_fixture = new Fixture();
			_caseId = _fixture.Create<int>().ToString();
			_onBehalfOfAccessToken = _fixture.Create<string>();
			_rumpolePipelineCoordinatorScope = _fixture.Create<string>();
			_tracker = _fixture.Create<Tracker>();

			_mockLogger = new Mock<ILogger<RumpolePipelineGetTracker>>();
			_mockOnBehalfOfTokenClient = new Mock<IOnBehalfOfTokenClient>();
			_mockPipelineClient = new Mock<IPipelineClient>();
			_mockConfiguration = new Mock<IConfiguration>();

			_mockOnBehalfOfTokenClient.Setup(client => client.GetAccessTokenAsync(It.IsAny<string>(), _rumpolePipelineCoordinatorScope))
				.ReturnsAsync(_onBehalfOfAccessToken);
			_mockPipelineClient.Setup(client => client.GetTrackerAsync(_caseId, _onBehalfOfAccessToken))
				.ReturnsAsync(_tracker);
			_mockConfiguration.Setup(config => config["RumpolePipelineCoordinatorScope"]).Returns(_rumpolePipelineCoordinatorScope);

			RumpolePipelineGetTracker = new RumpolePipelineGetTracker(_mockLogger.Object, _mockOnBehalfOfTokenClient.Object, _mockPipelineClient.Object, _mockConfiguration.Object);
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
        {
			var response = await RumpolePipelineGetTracker.Run(CreateHttpRequestWithoutToken(), _caseId);

			response.Should().BeOfType<UnauthorizedObjectResult>();
        }

		[Fact]
		public async Task Run_ReturnsBadRequestWhenCaseIdIsNotAnInteger()
		{
			var response = await RumpolePipelineGetTracker.Run(CreateHttpRequest(), "Not an integer");

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsNotFoundWhenPipelineClientReturnsNull()
		{
			_mockPipelineClient.Setup(client => client.GetTrackerAsync(_caseId, _onBehalfOfAccessToken))
				.ReturnsAsync(default(Tracker));

			var response = await RumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId);

			response.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await RumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId);

			response.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsTracker()
		{
			var response = await RumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId) as OkObjectResult;

			response.Value.Should().Be(_tracker);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenMsalExceptionOccurs()
        {
			_mockOnBehalfOfTokenClient.Setup(client => client.GetAccessTokenAsync(It.IsAny<string>(), _rumpolePipelineCoordinatorScope))
				.ThrowsAsync(new MsalException());

			var response = await RumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenHttpExceptionOccurs()
		{
			_mockPipelineClient.Setup(client => client.GetTrackerAsync(_caseId, _onBehalfOfAccessToken))
				.ThrowsAsync(new HttpRequestException());

			var response = await RumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_mockPipelineClient.Setup(client => client.GetTrackerAsync(_caseId, _onBehalfOfAccessToken))
				.ThrowsAsync(new Exception());

			var response = await RumpolePipelineGetTracker.Run(CreateHttpRequest(), _caseId) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}
	}
}


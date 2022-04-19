using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Functions.RumpolePipeline;
using Xunit;

namespace RumpoleGateway.Tests.Functions.RumpolePipeline
{
	public class RumpolePipelineTriggerCoordinatorTests : SharedMethods.SharedMethods
	{
		private Fixture _fixture;
		private string _caseId;
		private string _onBehalfOfAccessToken;
		private HttpResponseMessage _httpResponseMessage;

		private Mock<ILogger<RumpolePipelineTriggerCoordinator>> _mockLogger;
		private Mock<IOnBehalfOfTokenClient> _mockOnBehalfOfTokenClient;
		private Mock<IPipelineClient> _mockPipelineClient;
		private Mock<IConfiguration> _mockConfiguration;

		private RumpolePipelineTriggerCoordinator RumpolePipelineTriggerCoordinator;

		public RumpolePipelineTriggerCoordinatorTests()
		{
			_fixture = new Fixture();
			_caseId = _fixture.Create<int>().ToString();
			_onBehalfOfAccessToken = _fixture.Create<string>();
			_httpResponseMessage = new HttpResponseMessage();

			_mockLogger = new Mock<ILogger<RumpolePipelineTriggerCoordinator>>();
			_mockOnBehalfOfTokenClient = new Mock<IOnBehalfOfTokenClient>();
			_mockPipelineClient = new Mock<IPipelineClient>();
			_mockConfiguration = new Mock<IConfiguration>();

			_mockOnBehalfOfTokenClient.Setup(client => client.GetAccessToken(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(_onBehalfOfAccessToken);
			_mockPipelineClient.Setup(client => client.TriggerCoordinator(_caseId, _onBehalfOfAccessToken))
				.ReturnsAsync(_httpResponseMessage);

			RumpolePipelineTriggerCoordinator = new RumpolePipelineTriggerCoordinator(_mockLogger.Object, _mockOnBehalfOfTokenClient.Object, _mockPipelineClient.Object, _mockConfiguration.Object);
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
		{
			var response = await RumpolePipelineTriggerCoordinator.Run(CreateHttpRequestWithoutToken(), _caseId);

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task Run_ReturnsBadRequestWhenCaseIdIsNotAnInteger()
		{
			var response = await RumpolePipelineTriggerCoordinator.Run(CreateHttpRequest(), "Not an integer");

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await RumpolePipelineTriggerCoordinator.Run(CreateHttpRequest(), _caseId);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Run_ReturnsHttpResponseMessage()
		{
			var response = await RumpolePipelineTriggerCoordinator.Run(CreateHttpRequest(), _caseId);

			response.Should().Be(_httpResponseMessage);
		}
	}
}


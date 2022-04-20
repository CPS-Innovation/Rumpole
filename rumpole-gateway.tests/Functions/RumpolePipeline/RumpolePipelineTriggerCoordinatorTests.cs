using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
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
		private string _rumpolePipelineCoordinatorScope;
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
			_rumpolePipelineCoordinatorScope = _fixture.Create<string>();
			_httpResponseMessage = new HttpResponseMessage();

			_mockLogger = new Mock<ILogger<RumpolePipelineTriggerCoordinator>>();
			_mockOnBehalfOfTokenClient = new Mock<IOnBehalfOfTokenClient>();
			_mockPipelineClient = new Mock<IPipelineClient>();
			_mockConfiguration = new Mock<IConfiguration>();

			_mockOnBehalfOfTokenClient.Setup(client => client.GetAccessTokenAsync(It.IsAny<string>(), _rumpolePipelineCoordinatorScope))
				.ReturnsAsync(_onBehalfOfAccessToken);
			_mockPipelineClient.Setup(client => client.TriggerCoordinatorAsync(_caseId, _onBehalfOfAccessToken))
				.ReturnsAsync(_httpResponseMessage);
			_mockConfiguration.Setup(config => config["RumpolePipelineCoordinatorScope"]).Returns(_rumpolePipelineCoordinatorScope);

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

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenMsalExceptionOccurs()
		{
			_mockOnBehalfOfTokenClient.Setup(client => client.GetAccessTokenAsync(It.IsAny<string>(), _rumpolePipelineCoordinatorScope))
				.ThrowsAsync(new MsalException());
			var response = await RumpolePipelineTriggerCoordinator.Run(CreateHttpRequest(), _caseId);

			response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenHttpExceptionOccurs()
		{
			_mockPipelineClient.Setup(client => client.TriggerCoordinatorAsync(_caseId, _onBehalfOfAccessToken))
				.ThrowsAsync(new HttpRequestException());
			var response = await RumpolePipelineTriggerCoordinator.Run(CreateHttpRequest(), _caseId);

			response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_mockPipelineClient.Setup(client => client.TriggerCoordinatorAsync(_caseId, _onBehalfOfAccessToken))
				.ThrowsAsync(new Exception());
			var response = await RumpolePipelineTriggerCoordinator.Run(CreateHttpRequest(), _caseId);

			response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
		}
	}
}


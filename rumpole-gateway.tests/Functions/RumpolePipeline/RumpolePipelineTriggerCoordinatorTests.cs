using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Moq;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Factories;
using RumpoleGateway.Functions.RumpolePipeline;
using Xunit;

namespace RumpoleGateway.Tests.Functions.RumpolePipeline
{
	public class RumpolePipelineTriggerCoordinatorTests : SharedMethods.SharedMethods
	{
		private Fixture _fixture;
		private HttpRequest _request;
		private string _caseId;
		private string _onBehalfOfAccessToken;
		private string _rumpolePipelineCoordinatorScope;
		private TriggerCoordinatorResponse _triggerCoordinatorResponse;

		private Mock<ILogger<RumpolePipelineTriggerCoordinator>> _mockLogger;
		private Mock<IOnBehalfOfTokenClient> _mockOnBehalfOfTokenClient;
		private Mock<IPipelineClient> _mockPipelineClient;
		private Mock<IConfiguration> _mockConfiguration;
		private Mock<ITriggerCoordinatorResponseFactory> _mockTriggerCoordinatorResponseFactory;

		private RumpolePipelineTriggerCoordinator RumpolePipelineTriggerCoordinator;

		public RumpolePipelineTriggerCoordinatorTests()
		{
			_fixture = new Fixture();
			_caseId = _fixture.Create<int>().ToString();
			_onBehalfOfAccessToken = _fixture.Create<string>();
			_rumpolePipelineCoordinatorScope = _fixture.Create<string>();
			_request = CreateHttpRequest();

			_mockLogger = new Mock<ILogger<RumpolePipelineTriggerCoordinator>>();
			_mockOnBehalfOfTokenClient = new Mock<IOnBehalfOfTokenClient>();
			_mockPipelineClient = new Mock<IPipelineClient>();
			_mockConfiguration = new Mock<IConfiguration>();
			_mockTriggerCoordinatorResponseFactory = new Mock<ITriggerCoordinatorResponseFactory>();

			_mockOnBehalfOfTokenClient.Setup(client => client.GetAccessTokenAsync(It.IsAny<string>(), _rumpolePipelineCoordinatorScope))
				.ReturnsAsync(_onBehalfOfAccessToken);
			_mockConfiguration.Setup(config => config["RumpolePipelineCoordinatorScope"]).Returns(_rumpolePipelineCoordinatorScope);
			_mockTriggerCoordinatorResponseFactory.Setup(factory => factory.Create(_request)).Returns(_triggerCoordinatorResponse);

			RumpolePipelineTriggerCoordinator =
				new RumpolePipelineTriggerCoordinator(_mockLogger.Object, _mockOnBehalfOfTokenClient.Object, _mockPipelineClient.Object, _mockConfiguration.Object, _mockTriggerCoordinatorResponseFactory.Object);
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
		{
			var response = await RumpolePipelineTriggerCoordinator.Run(CreateHttpRequestWithoutToken(), _caseId);

			response.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsBadRequestWhenCaseIdIsNotAnInteger()
		{
			var response = await RumpolePipelineTriggerCoordinator.Run(_request, "Not an integer");

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_TriggersCoordinator()
        {
			var response = await RumpolePipelineTriggerCoordinator.Run(_request, _caseId);

			_mockPipelineClient.Verify(client => client.TriggerCoordinatorAsync(_caseId, _onBehalfOfAccessToken));
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await RumpolePipelineTriggerCoordinator.Run(_request, _caseId);

			response.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsTriggerCoordinatorResponse()
		{
			var response = await RumpolePipelineTriggerCoordinator.Run(_request, _caseId) as OkObjectResult;

			response.Value.Should().Be(_triggerCoordinatorResponse);
		
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenMsalExceptionOccurs()
		{
			_mockOnBehalfOfTokenClient.Setup(client => client.GetAccessTokenAsync(It.IsAny<string>(), _rumpolePipelineCoordinatorScope))
				.ThrowsAsync(new MsalException());

			var response = await RumpolePipelineTriggerCoordinator.Run(_request, _caseId) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenHttpExceptionOccurs()
		{
			_mockPipelineClient.Setup(client => client.TriggerCoordinatorAsync(_caseId, _onBehalfOfAccessToken))
				.ThrowsAsync(new HttpRequestException());

			var response = await RumpolePipelineTriggerCoordinator.Run(_request, _caseId) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_mockPipelineClient.Setup(client => client.TriggerCoordinatorAsync(_caseId, _onBehalfOfAccessToken))
				.ThrowsAsync(new Exception());

			var response = await RumpolePipelineTriggerCoordinator.Run(_request, _caseId) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}
	}
}


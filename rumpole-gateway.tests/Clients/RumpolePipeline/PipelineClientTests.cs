using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Factories;
using RumpoleGateway.Wrappers;
using Xunit;

namespace RumpoleGateway.Tests.Clients.RumpolePipeline
{
	public class PipelineClientTests
	{
		private Fixture _fixture;
		private string _caseId;
		private string _accessToken;
		private HttpRequestMessage _httpRequestMessage;
		private HttpResponseMessage _triggerCoordinatorHttpResponseMessage;
		private HttpResponseMessage _getTrackerHttpResponseMessage;
		private string _rumpolePipelineFunctionAppKey;
		private Tracker _tracker;

		private HttpClient _getTrackerHttpClient;
		private HttpClient _triggerCoordinatorHttpClient;
		private Mock<IRumpolePipelineRequestFactory> _mockRequestFactory;
		private Mock<IConfiguration> _mockConfiguration;
		private Mock<IJsonConvertWrapper> _mockJsonConvertWrapper;
		private Mock<ILogger<IPipelineClient>> _mockLogger;

		private PipelineClient TriggerCoordinatorPipelineClient;
		private PipelineClient GetTrackerPipelineClient;

		public PipelineClientTests()
		{
			_fixture = new Fixture();
			_caseId = _fixture.Create<string>();
			_accessToken = _fixture.Create<string>();
			_httpRequestMessage = new HttpRequestMessage();
			_tracker = _fixture.Create<Tracker>();
			_triggerCoordinatorHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
			_getTrackerHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(JsonConvert.SerializeObject(_tracker))
			};
			_rumpolePipelineFunctionAppKey = _fixture.Create<string>();

			var mockTriggerCoordinatorHttpMessageHandler = new Mock<HttpMessageHandler>();
			mockTriggerCoordinatorHttpMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", _httpRequestMessage, ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(_triggerCoordinatorHttpResponseMessage);
			_triggerCoordinatorHttpClient = new HttpClient(mockTriggerCoordinatorHttpMessageHandler.Object) { BaseAddress = new Uri("https://testUrl") };

			var mockTrackerHttpMessageHandler = new Mock<HttpMessageHandler>();
			mockTrackerHttpMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", _httpRequestMessage, ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(_getTrackerHttpResponseMessage);
			_getTrackerHttpClient = new HttpClient(mockTrackerHttpMessageHandler.Object) { BaseAddress = new Uri("https://testUrl") };

			_mockRequestFactory = new Mock<IRumpolePipelineRequestFactory>();
			_mockConfiguration = new Mock<IConfiguration>();
			_mockJsonConvertWrapper = new Mock<IJsonConvertWrapper>();
			_mockLogger = new Mock<ILogger<IPipelineClient>>();

			_mockConfiguration.Setup(config => config["RumpolePipelineFunctionAppKey"]).Returns(_rumpolePipelineFunctionAppKey);

			_mockRequestFactory.Setup(factory => factory.Create($"cases/{_caseId}?code={_rumpolePipelineFunctionAppKey}", _accessToken)).Returns(_httpRequestMessage);
			_mockRequestFactory.Setup(factory => factory.Create($"cases/{_caseId}/tracker?code={_rumpolePipelineFunctionAppKey}", _accessToken)).Returns(_httpRequestMessage);

			var stringContent = _getTrackerHttpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			_mockJsonConvertWrapper.Setup(wrapper => wrapper.DeserializeObject<Tracker>(stringContent)).Returns(_tracker);

			TriggerCoordinatorPipelineClient = new PipelineClient(_mockRequestFactory.Object, _triggerCoordinatorHttpClient, _mockConfiguration.Object, _mockJsonConvertWrapper.Object, _mockLogger.Object);
			GetTrackerPipelineClient = new PipelineClient(_mockRequestFactory.Object, _getTrackerHttpClient, _mockConfiguration.Object, _mockJsonConvertWrapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task TriggerCoordinator_ReturnsHttpResponseMessage()
        {
			var response = await TriggerCoordinatorPipelineClient.TriggerCoordinator(_caseId, _accessToken);

			response.Should().Be(_triggerCoordinatorHttpResponseMessage);
        }

		[Fact]
		public async Task TriggerCoordinator_ThrowsWhenHttpRequestExceptionOccurs()
		{
			_triggerCoordinatorHttpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;

			await Assert.ThrowsAsync<HttpRequestException>(() => TriggerCoordinatorPipelineClient.TriggerCoordinator(_caseId, _accessToken));
		}

		[Fact]
		public async Task GetTracker_ReturnsTracker()
		{
			var response = await GetTrackerPipelineClient.GetTracker(_caseId, _accessToken);

			response.Should().Be(_tracker);
		}

		[Fact]
		public async Task GetTracker_ReturnsNullWhenNotFoundStatusCodeReturned()
		{
			_getTrackerHttpResponseMessage.StatusCode = HttpStatusCode.NotFound;

			var response = await GetTrackerPipelineClient.GetTracker(_caseId, _accessToken);

			response.Should().BeNull();
		}

		[Fact]
		public async Task GetTracker_ThrowsWhenHttpRequestExceptionOccurs()
		{
			_getTrackerHttpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;

			await Assert.ThrowsAsync<HttpRequestException>(() => GetTrackerPipelineClient.GetTracker(_caseId, _accessToken));
		}
	}
}


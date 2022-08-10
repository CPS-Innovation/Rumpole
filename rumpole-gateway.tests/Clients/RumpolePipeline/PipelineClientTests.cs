using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
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
        private readonly string _caseId;
		private readonly string _accessToken;
		private readonly HttpRequestMessage _httpRequestMessage;
        private readonly HttpResponseMessage _getTrackerHttpResponseMessage;
		private readonly string _rumpolePipelineFunctionAppKey;
		private readonly Tracker _tracker;

        private readonly Mock<IPipelineClientRequestFactory> _mockRequestFactory;

        private readonly IPipelineClient _triggerCoordinatorPipelineClient;
		private readonly IPipelineClient _getTrackerPipelineClient;

		public PipelineClientTests()
		{
            var fixture = new Fixture();
			_caseId = fixture.Create<string>();
			_accessToken = fixture.Create<string>();
			_httpRequestMessage = new HttpRequestMessage();
			_tracker = fixture.Create<Tracker>();
			var triggerCoordinatorHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
			_getTrackerHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(JsonConvert.SerializeObject(_tracker))
			};
			_rumpolePipelineFunctionAppKey = fixture.Create<string>();

			var mockTriggerCoordinatorHttpMessageHandler = new Mock<HttpMessageHandler>();
			mockTriggerCoordinatorHttpMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", _httpRequestMessage, ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(triggerCoordinatorHttpResponseMessage);
			var triggerCoordinatorHttpClient = new HttpClient(mockTriggerCoordinatorHttpMessageHandler.Object) { BaseAddress = new Uri("https://testUrl") };

			var mockTrackerHttpMessageHandler = new Mock<HttpMessageHandler>();
			mockTrackerHttpMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", _httpRequestMessage, ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(_getTrackerHttpResponseMessage);
			var getTrackerHttpClient = new HttpClient(mockTrackerHttpMessageHandler.Object) { BaseAddress = new Uri("https://testUrl") };

			_mockRequestFactory = new Mock<IPipelineClientRequestFactory>();
			var mockConfiguration = new Mock<IConfiguration>();
			var mockJsonConvertWrapper = new Mock<IJsonConvertWrapper>();

			mockConfiguration.Setup(config => config["RumpolePipelineCoordinatorFunctionAppKey"]).Returns(_rumpolePipelineFunctionAppKey);

			_mockRequestFactory.Setup(factory => factory.CreateGet($"cases/{_caseId}?code={_rumpolePipelineFunctionAppKey}", _accessToken)).Returns(_httpRequestMessage);
			_mockRequestFactory.Setup(factory => factory.CreateGet($"cases/{_caseId}/tracker?code={_rumpolePipelineFunctionAppKey}", _accessToken)).Returns(_httpRequestMessage);

			var stringContent = _getTrackerHttpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			mockJsonConvertWrapper.Setup(wrapper => wrapper.DeserializeObject<Tracker>(stringContent)).Returns(_tracker);

			_triggerCoordinatorPipelineClient = new PipelineClient(_mockRequestFactory.Object, triggerCoordinatorHttpClient, mockConfiguration.Object, mockJsonConvertWrapper.Object);
			_getTrackerPipelineClient = new PipelineClient(_mockRequestFactory.Object, getTrackerHttpClient, mockConfiguration.Object, mockJsonConvertWrapper.Object);
		}

		[Fact]
		public async Task TriggerCoordinator_UrlHasNoForceQueryWhenForceIsFalse()
		{
			await _triggerCoordinatorPipelineClient.TriggerCoordinatorAsync(_caseId, _accessToken, false);

			_mockRequestFactory.Verify(factory => factory.CreateGet($"cases/{_caseId}?code={_rumpolePipelineFunctionAppKey}", _accessToken));
		}

		[Fact]
		public async Task TriggerCoordinator_UrlHasForceQueryWhenForceIsTrue()
		{
			var url = $"cases/{_caseId}?code={_rumpolePipelineFunctionAppKey}&&force=true";
			_mockRequestFactory.Setup(factory => factory.CreateGet(url, _accessToken)).Returns(_httpRequestMessage);

			await _triggerCoordinatorPipelineClient.TriggerCoordinatorAsync(_caseId, _accessToken, true);

			_mockRequestFactory.Verify(factory => factory.CreateGet(url, _accessToken));
		}

		[Fact]
		public async Task TriggerCoordinator_TriggersCoordinatorSuccessfully()
        {
			await _triggerCoordinatorPipelineClient.TriggerCoordinatorAsync(_caseId, _accessToken, false);
        }

		[Fact]
		public async Task GetTracker_ReturnsTracker()
		{
			var response = await _getTrackerPipelineClient.GetTrackerAsync(_caseId, _accessToken);

			response.Should().Be(_tracker);
		}

		[Fact]
		public async Task GetTracker_ReturnsNullWhenNotFoundStatusCodeReturned()
		{
			_getTrackerHttpResponseMessage.StatusCode = HttpStatusCode.NotFound;

			var response = await _getTrackerPipelineClient.GetTrackerAsync(_caseId, _accessToken);

			response.Should().BeNull();
		}
	}
}


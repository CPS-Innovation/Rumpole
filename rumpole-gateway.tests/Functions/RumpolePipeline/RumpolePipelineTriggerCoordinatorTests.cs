﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
using RumpoleGateway.Factories;
using RumpoleGateway.Functions.RumpolePipeline;
using Xunit;

namespace RumpoleGateway.Tests.Functions.RumpolePipeline
{
	public class RumpolePipelineTriggerCoordinatorTests : SharedMethods.SharedMethods
	{
        private readonly HttpRequest _request;
		private readonly string _caseId;
		private readonly string _onBehalfOfAccessToken;
		private readonly string _rumpolePipelineCoordinatorScope;
#pragma warning disable CS0649
		private TriggerCoordinatorResponse _triggerCoordinatorResponse;
#pragma warning restore CS0649

        private readonly Mock<IOnBehalfOfTokenClient> _mockOnBehalfOfTokenClient;
		private readonly Mock<IPipelineClient> _mockPipelineClient;

        private readonly RumpolePipelineTriggerCoordinator _rumpolePipelineTriggerCoordinator;

		public RumpolePipelineTriggerCoordinatorTests()
		{
            var fixture = new Fixture();
			_caseId = fixture.Create<int>().ToString();
			_onBehalfOfAccessToken = fixture.Create<string>();
			_rumpolePipelineCoordinatorScope = fixture.Create<string>();
			_request = CreateHttpRequest();

			var mockLogger = new Mock<ILogger<RumpolePipelineTriggerCoordinator>>();
			_mockOnBehalfOfTokenClient = new Mock<IOnBehalfOfTokenClient>();
			_mockPipelineClient = new Mock<IPipelineClient>();
			var mockConfiguration = new Mock<IConfiguration>();
			var mockTriggerCoordinatorResponseFactory = new Mock<ITriggerCoordinatorResponseFactory>();

			_mockOnBehalfOfTokenClient.Setup(client => client.GetAccessTokenAsync(It.IsAny<string>(), _rumpolePipelineCoordinatorScope))
				.ReturnsAsync(_onBehalfOfAccessToken);
			mockConfiguration.Setup(config => config["RumpolePipelineCoordinatorScope"]).Returns(_rumpolePipelineCoordinatorScope);
			mockTriggerCoordinatorResponseFactory.Setup(factory => factory.Create(_request)).Returns(_triggerCoordinatorResponse);

            var mockTokenValidator = new Mock<ITokenValidator>();

            mockTokenValidator.Setup(x => x.ValidateTokenAsync(It.IsAny<StringValues>())).ReturnsAsync(true);

            _rumpolePipelineTriggerCoordinator =
				new RumpolePipelineTriggerCoordinator(mockLogger.Object, _mockOnBehalfOfTokenClient.Object, _mockPipelineClient.Object, mockConfiguration.Object, mockTriggerCoordinatorResponseFactory.Object, mockTokenValidator.Object);
		}

		[Fact]
		public async Task Run_ReturnsUnauthorizedWhenAccessTokenIsMissing()
		{
			var response = await _rumpolePipelineTriggerCoordinator.Run(CreateHttpRequestWithoutToken(), _caseId);

			response.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsBadRequestWhenCaseIdIsNotAnInteger()
		{
			var response = await _rumpolePipelineTriggerCoordinator.Run(_request, "Not an integer");

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsBadRequestWhenForceIsNotABool()
		{
			_request.Query = new QueryCollection(new Dictionary<string, StringValues> { { "force", new StringValues("not a bool") } });
			var response = await _rumpolePipelineTriggerCoordinator.Run(_request, _caseId);

			response.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Run_TriggersCoordinator()
        {
			var response = await _rumpolePipelineTriggerCoordinator.Run(_request, _caseId);

			_mockPipelineClient.Verify(client => client.TriggerCoordinatorAsync(_caseId, _onBehalfOfAccessToken, false));
		}

		[Fact]
		public async Task Run_ReturnsOk()
		{
			var response = await _rumpolePipelineTriggerCoordinator.Run(_request, _caseId);

			response.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task Run_ReturnsTriggerCoordinatorResponse()
		{
			var response = await _rumpolePipelineTriggerCoordinator.Run(_request, _caseId) as OkObjectResult;

			response.Value.Should().Be(_triggerCoordinatorResponse);
		
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenMsalExceptionOccurs()
		{
			_mockOnBehalfOfTokenClient.Setup(client => client.GetAccessTokenAsync(It.IsAny<string>(), _rumpolePipelineCoordinatorScope))
				.ThrowsAsync(new MsalException());

			var response = await _rumpolePipelineTriggerCoordinator.Run(_request, _caseId) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenHttpExceptionOccurs()
		{
			_mockPipelineClient.Setup(client => client.TriggerCoordinatorAsync(_caseId, _onBehalfOfAccessToken, false))
				.ThrowsAsync(new HttpRequestException());

			var response = await _rumpolePipelineTriggerCoordinator.Run(_request, _caseId) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}

		[Fact]
		public async Task Run_ReturnsInternalServerErrorWhenUnhandledExceptionOccurs()
		{
			_mockPipelineClient.Setup(client => client.TriggerCoordinatorAsync(_caseId, _onBehalfOfAccessToken, false))
				.ThrowsAsync(new Exception());

			var response = await _rumpolePipelineTriggerCoordinator.Run(_request, _caseId) as StatusCodeResult;

			response.StatusCode.Should().Be(500);
		}
	}
}


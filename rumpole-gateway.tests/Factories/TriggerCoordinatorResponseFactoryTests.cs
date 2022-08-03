using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RumpoleGateway.Factories;
using RumpoleGateway.Mappers;
using Xunit;

namespace RumpoleGateway.Tests.Factories
{
	public class TriggerCoordinatorResponseFactoryTests
	{
		private HttpRequest _httpRequest;
		private Uri _trackerUrl;

		private Mock<ITrackerUrlMapper> _mockTrackerUrlMapper;

		private TriggerCoordinatorResponseFactory TriggerCoordinatorResponseFactory;

		public TriggerCoordinatorResponseFactoryTests()
		{
			var context = new DefaultHttpContext();
			_httpRequest = context.Request;
			_trackerUrl = new Uri("http://www.test.co.uk");

			_mockTrackerUrlMapper = new Mock<ITrackerUrlMapper>();

			_mockTrackerUrlMapper.Setup(mapper => mapper.Map(_httpRequest)).Returns(_trackerUrl);

			TriggerCoordinatorResponseFactory = new TriggerCoordinatorResponseFactory(_mockTrackerUrlMapper.Object);
		}

		[Fact]
		public void Map_ReturnsTrackerUrl()
		{
			var response = TriggerCoordinatorResponseFactory.Create(_httpRequest);

			response.TrackerUrl.Should().Be(_trackerUrl);
		}
	}
}


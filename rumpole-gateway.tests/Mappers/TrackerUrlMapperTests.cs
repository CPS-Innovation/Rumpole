using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RumpoleGateway.Mappers;
using Xunit;

namespace RumpoleGateway.Tests.Mappers
{
	public class TrackerUrlMapperTests
	{
		private Fixture _fixture;
		private string _requestScheme;
		private string _requestHost;
		private ushort _requestPort;

		private Mock<HttpRequest> _mockRequest;

		

		private TrackerUrlMapper TrackerUrlMapper;

		public TrackerUrlMapperTests()
        {
			_fixture = new Fixture();
			_requestScheme = "http";
			_requestHost = _fixture.Create<string>();
			_requestPort = _fixture.Create<ushort>();

			_mockRequest = new Mock<HttpRequest>();

			_mockRequest.Setup(x => x.Scheme).Returns(_requestScheme);
			_mockRequest.Setup(x => x.Host).Returns(new HostString(_requestHost, _requestPort));

			TrackerUrlMapper = new TrackerUrlMapper();
		}

		[Fact]
	    public void Map_ReturnsExpectedUriWhenDefinedPortIsSet()
        {
			var expectedAbsoluteUri = $"{_requestScheme}://{_requestHost}:{_requestPort}/tracker";

			var uri = TrackerUrlMapper.Map(_mockRequest.Object);

			uri.AbsoluteUri.Should().Be(expectedAbsoluteUri);
		}

		[Fact]
		public void Map_ReturnsExpectedUriWhenDefaultPortIsSet()
		{
			var expectedAbsoluteUri = $"{_requestScheme}://{_requestHost}/tracker";
			_mockRequest.Setup(x => x.Host).Returns(new HostString(_requestHost));

			var uri = TrackerUrlMapper.Map(_mockRequest.Object);

			uri.AbsoluteUri.Should().Be(expectedAbsoluteUri);
		}
	}
}


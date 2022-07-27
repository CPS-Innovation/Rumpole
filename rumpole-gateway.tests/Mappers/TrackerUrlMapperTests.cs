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
        private readonly string _requestScheme;
		private readonly string _requestHost;
		private readonly ushort _requestPort;

		private readonly Mock<HttpRequest> _mockRequest;

		private readonly TrackerUrlMapper _trackerUrlMapper;

		public TrackerUrlMapperTests()
        {
            var fixture = new Fixture();
			_requestScheme = "http";
			_requestHost = fixture.Create<string>();
			_requestPort = fixture.Create<ushort>();

			_mockRequest = new Mock<HttpRequest>();

			_mockRequest.Setup(x => x.Scheme).Returns(_requestScheme);
			_mockRequest.Setup(x => x.Host).Returns(new HostString(_requestHost, _requestPort));

			_trackerUrlMapper = new TrackerUrlMapper();
		}

		[Fact]
	    public void Map_ReturnsExpectedUriWhenDefinedPortIsSet()
        {
			var expectedAbsoluteUri = $"{_requestScheme}://{_requestHost}:{_requestPort}/tracker";

			var uri = _trackerUrlMapper.Map(_mockRequest.Object);

			uri.AbsoluteUri.Should().Be(expectedAbsoluteUri);
		}

		[Fact]
		public void Map_ReturnsExpectedUriWhenDefaultPortIsSet()
		{
			var expectedAbsoluteUri = $"{_requestScheme}://{_requestHost}/tracker";
			_mockRequest.Setup(x => x.Host).Returns(new HostString(_requestHost));

			var uri = _trackerUrlMapper.Map(_mockRequest.Object);

			uri.AbsoluteUri.Should().Be(expectedAbsoluteUri);
		}
	}
}


using System.Net.Http;
using AutoFixture;
using FluentAssertions;
using RumpoleGateway.Factories;
using Xunit;

namespace RumpoleGateway.Tests.Factories
{
    public class PipelineClientRequestFactoryTests
    {
        private readonly string _requestUri;
        private readonly string _accessToken;

        private readonly IPipelineClientRequestFactory _pipelineClientRequestFactory;

        public PipelineClientRequestFactoryTests()
        {
            var fixture = new Fixture();
            _requestUri = fixture.Create<string>();
            _accessToken = fixture.Create<string>();

            _pipelineClientRequestFactory = new PipelineClientRequestFactory();
        }

        [Fact]
        public void Create_SetsHttpMethodToGetOnRequestMessage()
        {
            var message = _pipelineClientRequestFactory.CreateGet(_requestUri, _accessToken);

            message.Method.Should().Be(HttpMethod.Get);
        }

        [Fact]
        public void Create_SetsRequestUriOnRequestMessage()
        {
            var message = _pipelineClientRequestFactory.CreateGet(_requestUri, _accessToken);

            message.RequestUri.Should().Be(_requestUri);
        }

        [Fact]
        public void Create_SetsAccessTokenOnRequestMessageAuthorizationHeader()
        {
            var message = _pipelineClientRequestFactory.CreateGet(_requestUri, _accessToken);

            message.Headers.Authorization?.ToString().Should().Be($"Bearer {_accessToken}");
        }
    }
}

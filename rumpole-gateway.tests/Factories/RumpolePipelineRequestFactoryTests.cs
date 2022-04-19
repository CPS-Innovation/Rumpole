using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using RumpoleGateway.Factories;
using Xunit;

namespace cms_document_services_document_processor_function.tests.Factories
{
    public class RumpolePipelineRequestFactoryTests
    {
        private Fixture _fixture;
        private string _requestUri;
        private string _accessToken;

        private IRumpolePipelineRequestFactory RumpolePipelineRequestFactory;

        public RumpolePipelineRequestFactoryTests()
        {
            _fixture = new Fixture();
            _requestUri = _fixture.Create<string>();
            _accessToken = _fixture.Create<string>();

            RumpolePipelineRequestFactory = new RumpolePipelineRequestFactory();
        }

        [Fact]
        public void Create_SetsHttpMethodToGetOnRequestMessage()
        {
            var message = RumpolePipelineRequestFactory.Create(_requestUri, _accessToken);

            message.Method.Should().Be(HttpMethod.Get);
        }

        [Fact]
        public void Create_SetsRequestUriOnRequestMessage()
        {
            var message = RumpolePipelineRequestFactory.Create(_requestUri, _accessToken);

            message.RequestUri.Should().Be(_requestUri);
        }

        [Fact]
        public void Create_SetsAccessTokenOnRequestMessageAuthorizationHeader()
        {
            var message = RumpolePipelineRequestFactory.Create(_requestUri, _accessToken);

            message.Headers.Authorization.ToString().Should().Be($"Bearer {_accessToken}");
        }
    }
}

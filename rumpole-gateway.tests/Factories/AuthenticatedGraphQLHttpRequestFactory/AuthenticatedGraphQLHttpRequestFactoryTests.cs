using System;
using AutoFixture;
using FluentAssertions;
using GraphQL.Client.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RumpoleGateway.Extensions;
using RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory;
using Xunit;

namespace RumpoleGateway.tests.Factories.AuthenticatedGraphQLHttpRequestFactory
{
    public class AuthenticatedGraphQlHttpRequestFactoryTests
    {
        [Fact]
        public void Create_CreatesAuthenticatedRequest()
        {
            var fixture = new Fixture();
            var correlationId = fixture.Create<Guid>();
            var loggerMock = new Mock<ILogger<AuthenticatedGraphQlHttpRequestFactory>>();
            
            var factory = new RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory.AuthenticatedGraphQlHttpRequestFactory(loggerMock.Object);

            var authenticatedRequest = factory.Create( "accessToken", new GraphQLHttpRequest(), correlationId);

            authenticatedRequest.Should().BeOfType<AuthenticatedGraphQlHttpRequest>();
        }
    }
}

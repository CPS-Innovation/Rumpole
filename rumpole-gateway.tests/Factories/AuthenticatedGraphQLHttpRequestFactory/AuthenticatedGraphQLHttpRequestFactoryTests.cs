using FluentAssertions;
using GraphQL.Client.Http;
using RumpoleGateway.Extensions;
using Xunit;

namespace RumpoleGateway.tests.Factories.AuthenticatedGraphQLHttpRequestFactory
{
    public class AuthenticatedGraphQLHttpRequestFactoryTests
    {
        [Fact]
        public void Create_CreatesAuthenticatedRequest()
        {
            var factory = new RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory.AuthenticatedGraphQLHttpRequestFactory();

            var authenticatedRequest = factory.Create( "accessToken", new GraphQLHttpRequest());

            authenticatedRequest.Should().BeOfType<AuthenticatedGraphQlHttpRequest>();
        }
    }
}

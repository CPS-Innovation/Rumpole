using GraphQL.Client.Http;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory
{
    public class AuthenticatedGraphQLHttpRequestFactory : IAuthenticatedGraphQLHttpRequestFactory
    {
        public AuthenticatedGraphQLHttpRequest Create(string accessToken, GraphQLHttpRequest graphQLHttpRequest)
        {
            return new AuthenticatedGraphQLHttpRequest(accessToken, graphQLHttpRequest);
        }
    }
}

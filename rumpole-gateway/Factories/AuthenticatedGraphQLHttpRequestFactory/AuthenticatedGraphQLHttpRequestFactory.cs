using GraphQL.Client.Http;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory
{
    public class AuthenticatedGraphQLHttpRequestFactory : IAuthenticatedGraphQLHttpRequestFactory
    {
        public AuthenticatedGraphQlHttpRequest Create(string accessToken, GraphQLHttpRequest graphQLHttpRequest)
        {
            return new AuthenticatedGraphQlHttpRequest(accessToken, graphQLHttpRequest);
        }
    }
}

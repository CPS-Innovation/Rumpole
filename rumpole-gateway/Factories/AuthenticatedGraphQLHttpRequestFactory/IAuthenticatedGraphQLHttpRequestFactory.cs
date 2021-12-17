using GraphQL.Client.Http;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory
{
    public interface IAuthenticatedGraphQLHttpRequestFactory
    {
        AuthenticatedGraphQLHttpRequest Create(string accessToken, GraphQLHttpRequest graphQLHttpRequest);
    }
}

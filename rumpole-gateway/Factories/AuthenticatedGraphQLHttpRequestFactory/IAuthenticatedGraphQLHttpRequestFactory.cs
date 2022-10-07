using System;
using GraphQL.Client.Http;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory
{
    public interface IAuthenticatedGraphQlHttpRequestFactory
    {
        AuthenticatedGraphQlHttpRequest Create(string accessToken, GraphQLHttpRequest graphQlHttpRequest, Guid correlationId);
    }
}

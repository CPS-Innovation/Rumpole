using System;
using GraphQL.Client.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory
{
    public class AuthenticatedGraphQlHttpRequestFactory : IAuthenticatedGraphQlHttpRequestFactory
    {
        private readonly ILogger<AuthenticatedGraphQlHttpRequestFactory> _logger;

        public AuthenticatedGraphQlHttpRequestFactory(ILogger<AuthenticatedGraphQlHttpRequestFactory> logger)
        {
            _logger = logger;
        }

        public AuthenticatedGraphQlHttpRequest Create(string accessToken, GraphQLHttpRequest graphQlHttpRequest, Guid correlationId)
        {
            _logger.LogMethodEntry(correlationId, nameof(Create), graphQlHttpRequest.ToJson());
            
            var authRequest = new AuthenticatedGraphQlHttpRequest(accessToken, graphQlHttpRequest);
            _logger.LogMethodExit(correlationId, nameof(Create), string.Empty);
            return authRequest;
        }
    }
}

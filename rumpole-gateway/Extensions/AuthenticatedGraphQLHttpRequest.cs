using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace RumpoleGateway.Extensions
{
    public class AuthenticatedGraphQLHttpRequest : GraphQLHttpRequest
    {
        private readonly string SessionHeaderName = "core-data-api-sessionid";

        private readonly string _accessToken;

        public AuthenticatedGraphQLHttpRequest(string accessToken, GraphQLHttpRequest request)
            : base(request)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentException("Parameter must be supplied.", nameof(accessToken));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _accessToken = accessToken;
        }

        public override HttpRequestMessage ToHttpRequestMessage(GraphQLHttpClientOptions options, IGraphQLJsonSerializer serializer)
        {
            var message = base.ToHttpRequestMessage(options, serializer);
            message.Headers.Add("Authorization", $"bearer {_accessToken}");
            message.Headers.Add(SessionHeaderName, _accessToken);

            return message;
        }
    }
}

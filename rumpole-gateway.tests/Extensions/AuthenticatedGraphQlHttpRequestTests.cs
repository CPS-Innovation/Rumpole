﻿using RumpoleGateway.Extensions;
using System;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Xunit;

namespace RumpoleGateway.Tests.Extensions
{
    public class AuthenticatedGraphQlHttpRequestTests
    {
        private readonly Fixture _fixture;

        public AuthenticatedGraphQlHttpRequestTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void WhenCallingANewInstance_WithValidParameters_ThenAValidObjectIsCreated()
        {
            var testInstance = new AuthenticatedGraphQlHttpRequest(_fixture.Create<string>(), _fixture.Create<GraphQLHttpRequest>());

            testInstance.Should().NotBeNull();
        }

        [Fact]
        public void WhenCallingANewInstance_WithoutAnAccessToken_ThenAnArgumentExceptionIsThrown()
        {
            var testInstance = () => new AuthenticatedGraphQlHttpRequest(null, _fixture.Create<GraphQLHttpRequest>());

            testInstance.Should().Throw<ArgumentException>().WithParameterName("accessToken");
        }

        [Fact]
        public void WhenCallingANewInstance_WithoutAnEmptyGraphQlRequest_ThenAnArgumentNullExceptionIsThrown()
        {
            var testInstance = () => new AuthenticatedGraphQlHttpRequest(_fixture.Create<string>(), null);

            testInstance.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenAValidInstanceHasBeenCreated_WhenCallingToHttpRequestMessage_TheResponseContainsTheExpectedHeaders()
        {
            var accessToken = _fixture.Create<string>();
            var testInstance = new AuthenticatedGraphQlHttpRequest(accessToken, _fixture.Create<GraphQLHttpRequest>());
            var testRequestMessage = testInstance.ToHttpRequestMessage(new GraphQLHttpClientOptions(), new NewtonsoftJsonSerializer());

            using (new AssertionScope())
            {
                testRequestMessage.Headers.Should().Contain(x => x.Key == Constants.Authentication.Authorization);
                
                var authHeaderValues = testRequestMessage.Headers.GetValues(Constants.Authentication.Authorization);
                authHeaderValues.Should().Contain(x => x == $"{Constants.Authentication.Bearer} {accessToken}");

                testRequestMessage.Headers.Should().Contain(x => x.Key == "Correlation-Id");
                testRequestMessage.Headers.Should().Contain(x => x.Key == "Request-Ip-Address");
            }
        }
    }
}

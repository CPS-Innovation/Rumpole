﻿using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using RumpoleGateway.Clients.CoreDataApi;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory;
using System;

[assembly: FunctionsStartup(typeof(RumpoleGateway.Startup))]

namespace RumpoleGateway
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .Build();


            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IGraphQLClient>(s => new GraphQLHttpClient(GetValueFromConfig(configuration, "CoreDataApiUrl"), new NewtonsoftJsonSerializer()));
            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddScoped<ICoreDataApiClient, CoreDataApiClient>();
            builder.Services.AddSingleton<IAuthenticatedGraphQLHttpRequestFactory, AuthenticatedGraphQLHttpRequestFactory>();


            builder.Services.AddSingleton(serviceProvider =>
            {
                var instance = Constants.Authentication.AzureAuthenticationInstanceUrl; 
                var onBehalfOfTokenTenantId = GetValueFromConfig(configuration, "OnBehalfOfTokenTenantId");
                var onBehalfOfTokenClientId = GetValueFromConfig(configuration, "OnBehalfOfTokenClientId");
                var onBehalfOfTokenClientSecret = GetValueFromConfig(configuration, "OnBehalfOfTokenClientSecret");
                var appOptions = new ConfidentialClientApplicationOptions
                {
                    Instance = instance,
                    TenantId = onBehalfOfTokenTenantId,
                    ClientId = onBehalfOfTokenClientId,
                    ClientSecret = onBehalfOfTokenClientSecret
                };

                var authority = $"{instance}{onBehalfOfTokenTenantId}/";

                return ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(appOptions).WithAuthority(authority).Build();
            });

            builder.Services.AddTransient<IOnBehalfOfTokenClient, OnBehalfOfTokenClient>();

        }

        private string GetValueFromConfig(IConfiguration configuration, string secretName)
        {
            var secret = configuration[secretName];
            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new Exception($"Secret cannot be null: {secretName}");
            }

            return secret;
        }
    }
}
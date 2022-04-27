using System;
using System.Net.Http.Headers;
using Azure.Identity;
using Azure.Storage.Blobs;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using RumpoleGateway.Clients.CoreDataApi;
using RumpoleGateway.Clients.DocumentExtraction;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Factories;
using RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory;
using RumpoleGateway.Mappers;
using RumpoleGateway.Wrappers;

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

            builder.Services.AddScoped<IGraphQLClient>(s => new GraphQLHttpClient(GetValueFromConfig(configuration, "CoreDataApiUrl"), new NewtonsoftJsonSerializer()));
            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddScoped<ICoreDataApiClient, CoreDataApiClient>();
            builder.Services.AddTransient<IAuthenticatedGraphQLHttpRequestFactory, AuthenticatedGraphQLHttpRequestFactory>();
            builder.Services.AddTransient<IOnBehalfOfTokenClient, OnBehalfOfTokenClient>();
            builder.Services.AddTransient<IPipelineClientRequestFactory, PipelineClientRequestFactory>();
            builder.Services.AddTransient<IJsonConvertWrapper, JsonConvertWrapper>();
            builder.Services.AddTransient<ITriggerCoordinatorResponseFactory, TriggerCoordinatorResponseFactory>();
            builder.Services.AddTransient<ITrackerUrlMapper, TrackerUrlMapper>();

            builder.Services.AddHttpClient<IPipelineClient, PipelineClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["RumpolePipelineCoordinatorBaseUrl"]);
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            });

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

            builder.Services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(new Uri(configuration["BlobServiceUrl"]))
                    .WithCredential(new DefaultAzureCredential());
            });
            builder.Services.AddTransient<IBlobStorageClient>(serviceProvider =>
            {
                return new BlobStorageClient(
                    serviceProvider.GetRequiredService<BlobServiceClient>(),
                    configuration["BlobServiceContainerName"]);
            });
            builder.Services.AddTransient<IDocumentExtractionClient>(serviceProvider =>
            {
                return new DocumentExtractionClientStub(
                    configuration["StubBlobStorageConnectionString"]);
            });

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

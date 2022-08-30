using System;
using System.Diagnostics.CodeAnalysis;
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
using RumpoleGateway.Clients.DocumentRedaction;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.Config;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Factories;
using RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory;
using RumpoleGateway.Mappers;
using RumpoleGateway.Services;
using RumpoleGateway.Wrappers;

[assembly: FunctionsStartup(typeof(RumpoleGateway.Startup))]

namespace RumpoleGateway
{
    [ExcludeFromCodeCoverage]
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .Build();

            builder.Services.AddOptions<SearchClientOptions>().Configure<IConfiguration>((settings, _) =>
            {
                configuration.GetSection("searchClient").Bind(settings);
            });
            builder.Services.AddOptions<BlobOptions>().Configure<IConfiguration>((settings, _) =>
            {
                configuration.GetSection("blob").Bind(settings);
            });

            builder.Services.AddScoped<IGraphQLClient>(_ => new GraphQLHttpClient(GetValueFromConfig(configuration, "CoreDataApiUrl"), new NewtonsoftJsonSerializer()));
            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddScoped<ICoreDataApiClient, CoreDataApiClient>();
            builder.Services.AddTransient<IAuthenticatedGraphQLHttpRequestFactory, AuthenticatedGraphQLHttpRequestFactory>();
            builder.Services.AddTransient<ITokenValidator, TokenValidator>();
            builder.Services.AddTransient<IOnBehalfOfTokenClient, OnBehalfOfTokenClient>();
            builder.Services.AddTransient<IPipelineClientRequestFactory, PipelineClientRequestFactory>();
            builder.Services.AddTransient<IJsonConvertWrapper, JsonConvertWrapper>();
            builder.Services.AddTransient<ITriggerCoordinatorResponseFactory, TriggerCoordinatorResponseFactory>();
            builder.Services.AddTransient<ITrackerUrlMapper, TrackerUrlMapper>();
            builder.Services.AddTransient<ISearchIndexClient, SearchIndexClient>();
            builder.Services.AddTransient<ISearchClientFactory, SearchClientFactory>();
            builder.Services.AddTransient<IStreamlinedSearchLineMapper, StreamlinedSearchLineMapper>();
            builder.Services.AddTransient<IStreamlinedSearchWordMapper, StreamlinedSearchWordMapper>();
            builder.Services.AddTransient<IStreamlinedSearchResultFactory, StreamlinedSearchResultFactory>();

            builder.Services.AddHttpClient<IPipelineClient, PipelineClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["RumpolePipelineCoordinatorBaseUrl"]);
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            });

            builder.Services.AddHttpClient<IRedactionClient, RedactionClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["RumpolePipelineRedactPdfBaseUrl"]);
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            });

            builder.Services.AddSingleton(_ =>
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

            builder.Services.AddTransient<IBlobStorageClient>(serviceProvider => new BlobStorageClient(
                serviceProvider.GetRequiredService<BlobServiceClient>(),
                configuration["BlobServiceContainerName"]));

            builder.Services.AddTransient<IDocumentExtractionClient>(serviceProvider => new DocumentExtractionClientStub(configuration["StubBlobStorageConnectionString"]));

            builder.Services.AddTransient<ISasGeneratorService, SasGeneratorService>();
            builder.Services.AddTransient<IBlobSasBuilderWrapper, BlobSasBuilderWrapper>();
            builder.Services.AddTransient<IBlobSasBuilderFactory, BlobSasBuilderFactory>();
            builder.Services.AddTransient<IBlobSasBuilderWrapperFactory, BlobSasBuilderWrapperFactory>();
            builder.Services.AddTransient<IDocumentRedactionClient, DocumentRedactionClientStub>();
            builder.Services.AddTransient<IRedactPdfRequestMapper, RedactPdfRequestMapper>();
        }

        private static string GetValueFromConfig(IConfiguration configuration, string secretName)
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

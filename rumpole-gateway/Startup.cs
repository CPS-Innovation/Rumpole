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
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using RumpoleGateway.Clients.CoreDataApi;
using RumpoleGateway.Clients.DocumentExtraction;
using RumpoleGateway.Clients.DocumentRedaction;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Domain.Validators;
using RumpoleGateway.Factories;
using RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory;
using RumpoleGateway.CoreDataImplementations.Tde.Clients;
using RumpoleGateway.CoreDataImplementations.Tde.Options;
using RumpoleGateway.CoreDataImplementations.Tde.Services;
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

            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddOptions<SearchClientOptions>().Configure<IConfiguration>((settings, _) =>
            {
                configuration.GetSection("searchClient").Bind(settings);
            });

            builder.Services.AddScoped<IGraphQLClient>(_ => new GraphQLHttpClient(GetValueFromConfig(configuration, ConfigurationKeys.CoreDataApiUrl), new NewtonsoftJsonSerializer()));
            builder.Services.AddScoped<ICoreDataApiClient, CoreDataApiClient>();
            builder.Services.AddTransient<IAuthenticatedGraphQLHttpRequestFactory, AuthenticatedGraphQLHttpRequestFactory>();
            builder.Services.AddTransient<IOnBehalfOfTokenClient, OnBehalfOfTokenClient>();
            builder.Services.AddTransient<IPipelineClientRequestFactory, PipelineClientRequestFactory>();
            builder.Services.AddTransient<IAuthorizationValidator, AuthorizationValidator>();
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
                client.BaseAddress = new Uri(GetValueFromConfig(configuration, ConfigurationKeys.PipelineCoordinatorBaseUrl));
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            });

            builder.Services.AddHttpClient<IRedactionClient, RedactionClient>(client =>
            {
                client.BaseAddress = new Uri(GetValueFromConfig(configuration, ConfigurationKeys.PipelineRedactPdfBaseUrl));
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            });

            builder.Services.AddSingleton(_ =>
            {
                const string instance = AuthenticationKeys.AzureAuthenticationInstanceUrl;
                var onBehalfOfTokenTenantId = GetValueFromConfig(configuration, ConfigurationKeys.TenantId);
                var onBehalfOfTokenClientId = GetValueFromConfig(configuration, ConfigurationKeys.ClientId);
                var onBehalfOfTokenClientSecret = GetValueFromConfig(configuration, ConfigurationKeys.ClientSecret);
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

            builder.Services.AddAzureClients(azureBuilder =>
            {
                azureBuilder.AddBlobServiceClient(new Uri(GetValueFromConfig(configuration, ConfigurationKeys.BlobServiceUrl)))
                    .WithCredential(new DefaultAzureCredential());
            });

            builder.Services.AddTransient<IBlobStorageClient>(serviceProvider =>
            {
                var logger = serviceProvider.GetService<ILogger<BlobStorageClient>>();
                return new BlobStorageClient(serviceProvider.GetRequiredService<BlobServiceClient>(),
                    GetValueFromConfig(configuration, ConfigurationKeys.BlobContainerName), logger);
            });

            builder.Services.AddTransient<IDocumentExtractionClient>(serviceProvider =>
            {
                var logger = serviceProvider.GetService<ILogger<DocumentExtractionClientStub>>();
                return new DocumentExtractionClientStub(GetValueFromConfig(configuration, ConfigurationKeys.StubBlobStorageConnectionString), logger);
            });
            builder.Services.AddTransient<ISasGeneratorService, SasGeneratorService>();
            builder.Services.AddTransient<IBlobSasBuilderWrapper, BlobSasBuilderWrapper>();
            builder.Services.AddTransient<IBlobSasBuilderFactory, BlobSasBuilderFactory>();
            builder.Services.AddTransient<IBlobSasBuilderWrapperFactory, BlobSasBuilderWrapperFactory>();
            builder.Services.AddTransient<IDocumentRedactionClient, DocumentRedactionClientStub>();
            builder.Services.AddTransient<IRedactPdfRequestMapper, RedactPdfRequestMapper>();

            builder.Services.AddTransient<ICaseDataServiceArgFactory, CaseDataServiceArgFactory>();

            builder.Services.AddOptions<TdeOptions>().Configure<IConfiguration>((settings, _) =>
            {
                configuration.GetSection("tde").Bind(settings);
            });

            builder.Services.AddTransient<ICaseDataService, TdeCaseDataService>();

            builder.Services.AddHttpClient<ITdeClient, TdeClient>((client) =>
            {
                var options = configuration.GetSection("tde").Get<TdeOptions>();
                client.BaseAddress = new Uri(GetValueFromConfig(configuration, options.BaseUrl));
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            });

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

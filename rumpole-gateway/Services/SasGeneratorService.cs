using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RumpoleGateway.Domain.Config;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Factories;

namespace RumpoleGateway.Services
{
    public class SasGeneratorService : ISasGeneratorService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IBlobSasBuilderFactory _blobSasBuilderFactory;
        private readonly IBlobSasBuilderWrapperFactory _blobSasBuilderWrapperFactory;
        private readonly BlobOptions _blobOptions;
        private readonly ILogger<SasGeneratorService> _logger;

        public SasGeneratorService(
            BlobServiceClient blobServiceClient,
            IBlobSasBuilderFactory blobSasBuilderFactory,
            IBlobSasBuilderWrapperFactory blobSasBuilderWrapperFactory,
            IOptions<BlobOptions> blobOptions, 
            ILogger<SasGeneratorService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _blobSasBuilderFactory = blobSasBuilderFactory ?? throw new ArgumentNullException(nameof(blobSasBuilderFactory));
            _blobSasBuilderWrapperFactory = blobSasBuilderWrapperFactory ?? throw new ArgumentNullException(nameof(blobSasBuilderWrapperFactory));
            _blobOptions = blobOptions != null ? blobOptions.Value : throw new ArgumentNullException(nameof(blobOptions));
            _logger = logger;
        }

        public async Task<string> GenerateSasUrlAsync(string blobName, Guid correlationId)
        {
            _logger.LogMethodEntry(correlationId, nameof(GenerateSasUrlAsync), $"For blob name: '{blobName}'");
            
            var now = DateTimeOffset.UtcNow;
            var userDelegationKey = await _blobServiceClient.GetUserDelegationKeyAsync(now, now.AddSeconds(_blobOptions.UserDelegationKeyExpirySecs));

            var blobUri = new Uri($"{_blobServiceClient.Uri}{_blobOptions.BlobContainerName}/{blobName}");
            var blobUriBuilder = new BlobUriBuilder(blobUri); 
            var sasBuilder = _blobSasBuilderFactory.Create(blobUriBuilder.BlobName, correlationId);
            var sasBuilderWrapper = _blobSasBuilderWrapperFactory.Create(sasBuilder, correlationId);        
            blobUriBuilder.Sas = sasBuilderWrapper.ToSasQueryParameters(userDelegationKey, _blobServiceClient.AccountName, correlationId);

            _logger.LogMethodEntry(correlationId, nameof(GenerateSasUrlAsync), string.Empty);
            return blobUriBuilder.ToUri().ToString();      
        }
    }
}

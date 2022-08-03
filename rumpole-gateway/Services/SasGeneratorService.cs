using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using RumpoleGateway.Domain.Config;
using RumpoleGateway.Factories;

namespace RumpoleGateway.Services
{
    public class SasGeneratorService : ISasGeneratorService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IBlobSasBuilderFactory _blobSasBuilderFactory;
        private readonly IBlobSasBuilderWrapperFactory _blobSasBuilderWrapperFactory;
        private readonly BlobOptions _blobOptions;

        public SasGeneratorService(
            BlobServiceClient blobServiceClient,
            IBlobSasBuilderFactory blobSasBuilderFactory,
            IBlobSasBuilderWrapperFactory blobSasBuilderWrapperFactory,
            IOptions<BlobOptions> blobOptions)
        {
            _blobServiceClient = blobServiceClient;
            _blobSasBuilderFactory = blobSasBuilderFactory ?? throw new ArgumentNullException(nameof(blobSasBuilderFactory));
            _blobSasBuilderWrapperFactory = blobSasBuilderWrapperFactory ?? throw new ArgumentNullException(nameof(blobSasBuilderWrapperFactory));
            _blobOptions = blobOptions != null ? blobOptions.Value : throw new ArgumentNullException(nameof(blobOptions));
        }

        public async Task<string> GenerateSasUrlAsync(string blobName)
        {
            var now = DateTimeOffset.UtcNow;
            var userDelegationKey = await _blobServiceClient.GetUserDelegationKeyAsync(now, now.AddSeconds(_blobOptions.UserDelegationKeyExpirySecs));

            var blobUri = new Uri($"{_blobServiceClient.Uri}{_blobOptions.BlobContainerName}/{blobName}");
            var blobUriBuilder = new BlobUriBuilder(blobUri); 
            var sasBuilder = _blobSasBuilderFactory.Create(blobUriBuilder.BlobName);
            var sasBuilderWrapper = _blobSasBuilderWrapperFactory.Create(sasBuilder);        
            blobUriBuilder.Sas = sasBuilderWrapper.ToSasQueryParameters(userDelegationKey, _blobServiceClient.AccountName);

            return blobUriBuilder.ToUri().ToString();      
        }
    }
}

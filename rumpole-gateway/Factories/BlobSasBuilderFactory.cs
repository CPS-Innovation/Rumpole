using System;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using RumpoleGateway.Domain.Config;

namespace RumpoleGateway.Factories
{
    public class BlobSasBuilderFactory : IBlobSasBuilderFactory
    {
        private readonly BlobOptions _blobOptions;

        public BlobSasBuilderFactory(IOptions<BlobOptions> blobOptions)
        {
            _blobOptions = blobOptions.Value;
        }

        public BlobSasBuilder Create(string blobName)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _blobOptions.BlobContainerName,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow
            };
            sasBuilder.ExpiresOn = sasBuilder.StartsOn.AddSeconds(_blobOptions.BlobExpirySecs);
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return sasBuilder;
        }
    }
}
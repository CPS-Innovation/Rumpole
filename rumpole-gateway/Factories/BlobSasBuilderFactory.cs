using System;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RumpoleGateway.Domain.Config;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Factories
{
    public class BlobSasBuilderFactory : IBlobSasBuilderFactory
    {
        private readonly ILogger<BlobSasBuilderFactory> _logger;
        private readonly BlobOptions _blobOptions;

        public BlobSasBuilderFactory(IOptions<BlobOptions> blobOptions, ILogger<BlobSasBuilderFactory> logger)
        {
            _blobOptions = blobOptions.Value;
            _logger = logger;
        }

        public BlobSasBuilder Create(string blobName, Guid correlationId)
        {
            _logger.LogMethodEntry(correlationId, nameof(Create), $"Blob Name: '{blobName}'");
            
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _blobOptions.BlobContainerName,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow
            };
            sasBuilder.ExpiresOn = sasBuilder.StartsOn.AddSeconds(_blobOptions.BlobExpirySecs);
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            sasBuilder.ContentType = "application/pdf";
            
            _logger.LogMethodExit(correlationId, nameof(Create), sasBuilder.ToJson());
            return sasBuilder;
        }
    }
}
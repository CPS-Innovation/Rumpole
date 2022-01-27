using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace Services.BlobStorageService
{
    public class BlobStorageService
    {
        private readonly BlobStorageOptions _options;
        public BlobStorageService(IOptions<BlobStorageOptions> options)
        {
            _options = options.Value;
        }

        public async Task<string> UploadAsync(Stream stream, string blobName, string contentType)
        {
            var blobClient = CreateBobClient(blobName);

            await blobClient.UploadAsync(stream, true);

            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _options.ContainerName,
                Resource = "c"
            };

            sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddDays(365 * 2);
            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

            var fileSuffix = Path.GetExtension(blobName);
            sasBuilder.ContentType = contentType;

            return blobClient.GenerateSasUri(sasBuilder).AbsoluteUri;
        }

        public async Task<MemoryStream> DownloadAsync(string blobName)
        {
            var blobClient = CreateBobClient(blobName);

            var ms = new MemoryStream();
            await blobClient.DownloadToAsync(ms);
            return ms;
        }

        private BlobClient CreateBobClient(string blobName)
        {
            return new BlobClient(_options.ConnectionString, "rumpole", blobName);
        }
    }
}
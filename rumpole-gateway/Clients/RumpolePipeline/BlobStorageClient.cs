using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;

namespace RumpoleGateway.Clients.RumpolePipeline
{
	public class BlobStorageClient : IBlobStorageClient
	{
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _blobServiceContainerName;

        public BlobStorageClient(BlobServiceClient blobServiceClient, string blobServiceContainerName)
        {
            _blobServiceClient = blobServiceClient;
            _blobServiceContainerName = blobServiceContainerName;
        }

        public async Task<Stream> GetDocumentAsync(string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobServiceContainerName);

            if (!await blobContainerClient.ExistsAsync())
            {
                throw new RequestFailedException((int)HttpStatusCode.NotFound, $"Blob container '{_blobServiceContainerName}' does not exist");
            }

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var blob = await blobClient.DownloadContentAsync();

            return blob.Value.Content.ToStream();
        }

        public async Task UploadDocumentAsync(Stream stream, string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobServiceContainerName);

            if (!await blobContainerClient.ExistsAsync())
            {
                throw new RequestFailedException((int)HttpStatusCode.NotFound, $"Blob container '{_blobServiceContainerName}' does not exist");
            }

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(stream, true);
        }
    }
}


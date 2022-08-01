using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace RumpoleGateway.Wrappers
{
    public class BlobSasBuilderWrapper : IBlobSasBuilderWrapper
    {
        private readonly BlobSasBuilder _blobSasBuilder;

        public BlobSasBuilderWrapper(BlobSasBuilder blobSasBuilder)
        {
            _blobSasBuilder = blobSasBuilder;
        }

        public BlobSasQueryParameters ToSasQueryParameters(UserDelegationKey userDelegationKey, string accountName)
        {
            return _blobSasBuilder.ToSasQueryParameters(userDelegationKey, accountName);
        }
    }
}

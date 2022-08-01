using Azure.Storage.Sas;
using RumpoleGateway.Wrappers;

namespace RumpoleGateway.Factories
{
    public class BlobSasBuilderWrapperFactory : IBlobSasBuilderWrapperFactory
    {
        public IBlobSasBuilderWrapper Create(BlobSasBuilder blobSasBuilder)
        {
            return new BlobSasBuilderWrapper(blobSasBuilder);
        }
    }
}

using Azure.Storage.Sas;
using FluentAssertions;
using RumpoleGateway.Factories;
using RumpoleGateway.Wrappers;
using Xunit;

namespace RumpoleGateway.Tests.Factories
{
    public class BlobSasBuilderWrapperFactoryTests
    {
        [Fact]
        public void Create_ReturnsBlobSasBuilderWrapper()
        {
            var factory = new BlobSasBuilderWrapperFactory();

            var blobSasBuilder = factory.Create(new BlobSasBuilder());

            blobSasBuilder.Should().BeOfType<BlobSasBuilderWrapper>();
        }
    }
}

using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RumpoleGateway.Domain.Config;
using RumpoleGateway.Factories;
using Xunit;

namespace RumpoleGateway.Tests.Factories
{
    public class BlobSasBuilderFactoryTests
    {
        private readonly BlobOptions _blobOptions;
        private readonly string _blobName;
        private Guid _correlationId;

        private readonly IBlobSasBuilderFactory _blobSasBuilderFactory;

        public BlobSasBuilderFactoryTests()
        {
            var fixture = new Fixture();
            _blobOptions = fixture.Create<BlobOptions>();
            _blobName = fixture.Create<string>();
            _correlationId = fixture.Create<Guid>();

            var mockBlobOptions = new Mock<IOptions<BlobOptions>>();
            var loggerMock = new Mock<ILogger<BlobSasBuilderFactory>>();

            mockBlobOptions.Setup(options => options.Value).Returns(_blobOptions);

            _blobSasBuilderFactory = new BlobSasBuilderFactory(mockBlobOptions.Object, loggerMock.Object);
        }

        [Fact]
        public void Create_ReturnsSasBuilderWithExpectedBlobContainerName()
        {
            var sasBuilder = _blobSasBuilderFactory.Create(_blobName, _correlationId);

            sasBuilder.BlobContainerName.Should().Be(_blobOptions.BlobContainerName);
        }

        [Fact]
        public void Create_ReturnsSasBuilderWithExpectedBlobName()
        {
            var sasBuilder = _blobSasBuilderFactory.Create(_blobName, _correlationId);

            sasBuilder.BlobName.Should().Be(_blobName);
        }

        [Fact]
        public void Create_ReturnsSasBuilderWithExpectedResource()
        {
            var sasBuilder = _blobSasBuilderFactory.Create(_blobName, _correlationId);

            sasBuilder.Resource.Should().Be("b");
        }

        [Fact]
        public void Create_ReturnsSasBuilderWithStartTimeBeforeNow()
        {
            var sasBuilder = _blobSasBuilderFactory.Create(_blobName, _correlationId);

            sasBuilder.StartsOn.Should().BeBefore(DateTimeOffset.UtcNow);
        }

        [Fact]
        public void Create_ReturnsSasBuilderWithExpectedExpiresOn()
        {
            var sasBuilder = _blobSasBuilderFactory.Create(_blobName, _correlationId);

            sasBuilder.ExpiresOn.Should().Be(sasBuilder.StartsOn.AddSeconds(_blobOptions.BlobExpirySecs));
        }

        [Fact]
        public void Create_ReturnsSasBuilderWithExpectedPermissions()
        {
            var sasBuilder = _blobSasBuilderFactory.Create(_blobName, _correlationId);

            sasBuilder.Permissions.Should().Be("r");
        }

        [Fact]
        public void Create_ReturnsSasBuilderWithExpectedContentType()
        {
            var sasBuilder = _blobSasBuilderFactory.Create(_blobName, _correlationId);

            sasBuilder.ContentType.Should().Be("application/pdf");
        }
    }
}

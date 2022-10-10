using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RumpoleGateway.Factories;
using Xunit;

namespace RumpoleGateway.Tests.Factories
{
    public class BlobSasBuilderFactoryTests
    {
        private readonly string _blobName;
        private readonly string _blobContainerName;
        private readonly int _blobExpirySecs;
        private readonly int _blobUserDelegationKeyExpirySecs;
        private readonly Guid _correlationId;

        private readonly IBlobSasBuilderFactory _blobSasBuilderFactory;

        public BlobSasBuilderFactoryTests()
        {
            var fixture = new Fixture();
            _blobName = fixture.Create<string>();
            _blobContainerName = fixture.Create<string>();
            _blobExpirySecs = fixture.Create<int>();
            _blobUserDelegationKeyExpirySecs = fixture.Create<int>();
            _correlationId = fixture.Create<Guid>();

            var mockConfiguration = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<BlobSasBuilderFactory>>();
            
            mockConfiguration.Setup(config => config["BlobContainerName"]).Returns(_blobContainerName);
            mockConfiguration.Setup(config => config["BlobName"]).Returns(_blobContainerName);
            mockConfiguration.Setup(config => config["BlobExpirySecs"]).Returns(_blobExpirySecs.ToString());
            mockConfiguration.Setup(config => config["BlobUserDelegationKeyExpirySecs"]).Returns(_blobUserDelegationKeyExpirySecs.ToString());

            _blobSasBuilderFactory = new BlobSasBuilderFactory(mockConfiguration.Object, loggerMock.Object);
        }

        [Fact]
        public void Create_ReturnsSasBuilderWithExpectedBlobContainerName()
        {
            var sasBuilder = _blobSasBuilderFactory.Create(_blobName, _correlationId);

            sasBuilder.BlobContainerName.Should().Be(_blobContainerName);
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

            sasBuilder.ExpiresOn.Should().Be(sasBuilder.StartsOn.AddSeconds(_blobExpirySecs));
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

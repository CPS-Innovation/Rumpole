using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Moq;
using RumpoleGateway.Clients.RumpolePipeline;
using Xunit;

namespace RumpoleGateway.Tests.Clients.RumpolePipeline
{
	public class BlobStorageClientTests
	{
		private Fixture _fixture;
		private string _blobContainerName;
		private string _blobName;
		private BlobDownloadResult _blobDownloadResult;
		private BinaryData _binaryData;

		private Mock<BlobServiceClient> _mockBlobServiceClient;
		private Mock<BlobContainerClient> _mockBlobContainerClient;
		private Mock<Response<bool>> _mockBlobContainerExistsResponse;
		private Mock<BlobClient> _mockBlobClient;
		private Mock<Response<bool>> _mockBlobClientExistsResponse;
		private Mock<Response<BlobDownloadResult>> _mockBlobDownloadResponse;

		private IBlobStorageClient BlobStorageClient;

		public BlobStorageClientTests()
		{
			_fixture = new Fixture();
			_blobContainerName = _fixture.Create<string>();
			_blobName = _fixture.Create<string>();
			_binaryData = new BinaryData(new byte[] { });

			_mockBlobServiceClient = new Mock<BlobServiceClient>();
			_mockBlobContainerClient = new Mock<BlobContainerClient>();
			_mockBlobClient = new Mock<BlobClient>();
			_mockBlobDownloadResponse = new Mock<Response<BlobDownloadResult>>();
			_blobDownloadResult = BlobsModelFactory.BlobDownloadResult(_binaryData);

			_mockBlobServiceClient.Setup(client => client.GetBlobContainerClient(_blobContainerName))
				.Returns(_mockBlobContainerClient.Object);

			_mockBlobContainerExistsResponse = new Mock<Response<bool>>();
			_mockBlobContainerExistsResponse.Setup(response => response.Value).Returns(true);
			_mockBlobContainerClient.Setup(client => client.ExistsAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(_mockBlobContainerExistsResponse.Object);
			_mockBlobContainerClient.Setup(client => client.GetBlobClient(_blobName)).Returns(_mockBlobClient.Object);

			_mockBlobClientExistsResponse = new Mock<Response<bool>>();
			_mockBlobClientExistsResponse.Setup(response => response.Value).Returns(true);
			_mockBlobClient.Setup(client => client.ExistsAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(_mockBlobClientExistsResponse.Object);
			_mockBlobClient.Setup(client => client.DownloadContentAsync()).ReturnsAsync(_mockBlobDownloadResponse.Object);

			_mockBlobDownloadResponse.Setup(response => response.Value).Returns(_blobDownloadResult);

			BlobStorageClient = new BlobStorageClient(_mockBlobServiceClient.Object, _blobContainerName);
		}

		[Fact]
		public async Task GetDocumentAsync_ThrowsRequestFailedExceptionWhenBlobContainerDoesNotExist()
        {
			_mockBlobContainerExistsResponse.Setup(response => response.Value).Returns(false);

			await Assert.ThrowsAsync<RequestFailedException>(() => BlobStorageClient.GetDocumentAsync(_blobName));
		}

		[Fact]
		public async Task GetDocumentAsync_ReturnsNullWhenBlobDoesNotExist()
		{
			_mockBlobClientExistsResponse.Setup(response => response.Value).Returns(false);

			var document = await BlobStorageClient.GetDocumentAsync(_blobName);

			document.Should().BeNull();
		}

		[Fact]
		public async Task GetDocumentAsync_ReturnsBlobStream()
		{
			var document = await BlobStorageClient.GetDocumentAsync(_blobName);

			document.Should().NotBeNull();
		}
	}
}


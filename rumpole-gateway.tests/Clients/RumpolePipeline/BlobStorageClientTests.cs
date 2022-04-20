using System;
using System.IO;
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
		private Stream _blobStream; 

		private Mock<BlobServiceClient> _mockBlobServiceClient;
		private Mock<BlobContainerClient> _mockBlobContainerClient;
		private Mock<Response<bool>> _mockBlobContainerExistsResponse;
		private Mock<BlobClient> _mockBlobClient;
		private Mock<Response<bool>> _mockBlobClientExistsResponse;
		private Mock<Response<BlobDownloadResult>> _mockBlobDownloadResponse;
		private Mock<BlobDownloadResult> _mockBlobDownloadResult;
		private Mock<BinaryData> _mockBinaryData;

		private IBlobStorageClient BlobStorageClient;

		public BlobStorageClientTests()
		{
			_fixture = new Fixture();
			_blobContainerName = _fixture.Create<string>();
			_blobName = _fixture.Create<string>();
			_blobStream = new MemoryStream();

			_mockBlobServiceClient = new Mock<BlobServiceClient>();
			_mockBlobContainerClient = new Mock<BlobContainerClient>();
			_mockBlobClient = new Mock<BlobClient>();
			_mockBlobDownloadResponse = new Mock<Response<BlobDownloadResult>>();
			_mockBinaryData = new Mock<BinaryData>();

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

			//_mockBlobDownloadResponse.Setup(response => response.Value).Returns(BlobsModelFactory.BlobDownloadResult);

			BlobStorageClient = new BlobStorageClient(_mockBlobServiceClient.Object, _blobContainerName);
		}

		[Fact]
		public async Task GetDocumentAsync_ThrowsRequestFailedExceptionWhenBlobContainerDoesNotExist()
        {

        }

		[Fact]
		public async Task GetDocumentAsync_ReturnsNullWhenBlobDoesNotExist()
		{

		}

		[Fact]
		public async Task GetDocumentAsync_ReturnsBlobStream()
		{
			//var blobStream = await BlobStorageClient.GetDocumentAsync(_blobName);

			//blobStream.Should().BeSameAs(_blobStream);
		}
	}
}


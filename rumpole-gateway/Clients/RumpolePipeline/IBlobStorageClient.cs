using System.IO;
using System.Threading.Tasks;

namespace RumpoleGateway.Clients.RumpolePipeline
{
	public interface IBlobStorageClient
	{
		Task<Stream> GetDocumentAsync(string blobName);

        Task UploadDocumentAsync(Stream stream, string blobName);
    }
}


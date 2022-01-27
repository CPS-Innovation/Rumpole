using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.BlobStorageService;

using Services.PngService;

namespace Functions.ProcessDocument
{
    public class PdfToPng
    {
        private readonly PngService _pngService;

        private readonly BlobStorageService _blobStorageService;

        public PdfToPng(PngService pngService, BlobStorageService blobStorageService)
        {
            _pngService = pngService;
            _blobStorageService = blobStorageService;
        }

        [FunctionName("pdf-to-png")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req, ILogger log)
        {
            try
            {
                var content = await req.Content.ReadAsStringAsync();
                var arg = JsonConvert.DeserializeObject<PdfToPngsArg>(content);

                var pdfStream = await _blobStorageService.DownloadAsync(arg.BlobName);
                var pngStreams = _pngService.GetPngStreams(pdfStream);

                var folderName = $"{arg.CaseId}/pngs/{arg.DocumentId}";

                var pngUploadTasks = pngStreams.Select((pngStream, index) => UploadBlobAsync(pngStream, $"{folderName}/{index}.png", "image/png"));

                var blobDetails = await Task.WhenAll(pngUploadTasks);

                var result = blobDetails.Select(blobDetails => new BlobNameAndSasLinkUrl
                {
                    BlobName = blobDetails.Item1,
                    SasLinkUrl = blobDetails.Item2
                }).ToList();

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                throw;
                // return new ObjectResult(ex.Message)
                // {
                //     StatusCode = 500
                // };
            }

        }

        private async Task<(string, string)> UploadBlobAsync(Stream stream, string blobName, string contentType)
        {
            var sasLink = await _blobStorageService.UploadAsync(stream, blobName, contentType);

            return (blobName, sasLink);
        }
    }
}
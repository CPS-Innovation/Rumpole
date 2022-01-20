using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.BlobStorageService;
using Services.CmsService;
using Services.PdfService;
using Services.PngService;

namespace Functions.ProcessDocument
{
  public class TransformAndSplit
  {
    private readonly CmsService _cmsService;

    private readonly PdfService _pdfService;

    private readonly PngService _pngService;

    private readonly BlobStorageService _blobStorageService;


    public TransformAndSplit(CmsService cmsService, PdfService pdfService, BlobStorageService blobStorageService, PngService pngService)
    {
      _cmsService = cmsService;
      _pdfService = pdfService;
      _blobStorageService = blobStorageService;
      _pngService = pngService;
    }

    [FunctionName("TransformAndSplit")]
    public async Task<TransformAndSplitResult> Run([ActivityTrigger] TransformAndSplitArg transformAndSplitArgs, ILogger log)
    {
      var url = transformAndSplitArgs.DocumentUrl;
      var caseId = transformAndSplitArgs.CaseId;
      var documentId = transformAndSplitArgs.DocumentId;

      log.LogWarning($"Getting {url}");
      var cmsDocument = await _cmsService.GetDocument(url);
      log.LogWarning($"Got {url}");

      var pdfStream = await _pdfService.GetPdfStream(cmsDocument.Stream, cmsDocument.ContentType.ToString());
      log.LogWarning($"Got pdf for {url}");

      var pngStreams = _pngService.GetPngTasks(pdfStream);

      log.LogWarning($"Got png streams for {url}");

      var folderName = $"{caseId}/{documentId}";
      var pdfBlobName = $"{folderName}.pdf";

      var pdfUploadTask = UploadBlobAsync(pdfStream, pdfBlobName, "application/pdf");
      var pngUploadTasks = pngStreams.Select((pngStream, index) => UploadBlobAsync(pngStream, $"{folderName}/{index}.png", "image/png"));

      var blobDetails = await Task.WhenAll(pngUploadTasks.Append(pdfUploadTask));

      log.LogWarning($"Uploaded pdf and pngs for {url}");

      var pngBlobDetails = blobDetails.Take(blobDetails.Length - 1);
      var pdfBlobDetails = blobDetails.Last();

      return new TransformAndSplitResult
      {
        Pdf = new BlobNameAndSasLinkUrl
        {
          BlobName = pdfBlobDetails.Item1,
          SasLinkUrl = pdfBlobDetails.Item2
        },
        Pngs = pngBlobDetails.Select(blobDetails => new BlobNameAndSasLinkUrl
        {
          BlobName = blobDetails.Item1,
          SasLinkUrl = blobDetails.Item2
        }).ToList()
      };
    }

    private async Task<(string, string)> UploadBlobAsync(Stream stream, string blobName, string contentType)
    {
      var sasLink = await _blobStorageService.UploadAsync(stream, blobName, contentType);

      return (blobName, sasLink);
    }
  }
}
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Docnet.Core;
using Docnet.Core.Converters;
using Docnet.Core.Models;

namespace Services.PngService
{
  public class PngService
  {
    private readonly IDocLib _docnet;
    public PngService(DocNetInstance docnetInstance)
    {
      _docnet = docnetInstance.GetDocNet();
    }

    public List<Stream> GetPngTasks(MemoryStream stream)
    {
      using var docReader = _docnet.GetDocReader(
              stream.ToArray(),
              new PageDimensions(1080, 1920));

      var pageCount = docReader.GetPageCount();

      var pngStreams = new List<Stream>();
      for (int i = 0; i < pageCount; i++)
      {
        using var pageReader = docReader.GetPageReader(i);

        var rawBytes = pageReader.GetImage();
        var remover = new NaiveTransparencyRemover();
        remover.Convert(rawBytes);

        var width = pageReader.GetPageWidth();
        var height = pageReader.GetPageHeight();

        using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

        AddBytes(bmp, rawBytes);

        var pngStream = new MemoryStream();

        bmp.Save(pngStream, ImageFormat.Png);
        pngStream.Seek(0, SeekOrigin.Begin);

        pngStreams.Add(pngStream);
      }

      return pngStreams;
    }

    private void AddBytes(Bitmap bmp, byte[] rawBytes)
    {
      var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

      var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
      var pNative = bmpData.Scan0;

      Marshal.Copy(rawBytes, 0, pNative, rawBytes.Length);
      bmp.UnlockBits(bmpData);
    }
  }

}
/*
      public static async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
          ILogger log)
      {
        string pdfBlobName = req.Query["blob-name"];

        var bytes = await DownloadAsync(pdfBlobName);

        var docnet = new ExampleFixture().DocNet;

        using var docReader = docnet.GetDocReader(
            bytes,
            new PageDimensions(1080, 1920));


        var pageCount = docReader.GetPageCount();

        var links = new List<Links>();

        for (int i = 0; i < pageCount; i++)
        {
          using var pageReader = docReader.GetPageReader(i);

          var rawBytes = pageReader.GetImage();
          var remover = new NaiveTransparencyRemover();
          remover.Convert(rawBytes);

          var width = pageReader.GetPageWidth();
          var height = pageReader.GetPageHeight();

          using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

          AddBytes(bmp, rawBytes);

          var stream = new MemoryStream();

          bmp.Save(stream, ImageFormat.Png);
          stream.Seek(0, SeekOrigin.Begin);

          var processedResult = await ProcessPng(pdfBlobName, i, stream);
          links.Add(processedResult);
          //await Task.Delay(500);
        }

        dynamic result = new ExpandoObject();
        result.pngUrls = links.Select(sasLink => sasLink.PngUrl);
        result.jsonUrls = links.Select(sasLink => sasLink.JsonUrl);
        return new JsonResult(result);
      }


      private static async Task<Links> ProcessPng(string blobName, int i, Stream pngStream)
      {
        var folderName = blobName.Replace(".pdf", "");
        var pngName = $"{folderName}/png/{i}.png";

        var pngSasLink = await UploadAsync(pngName, pngStream);

        var jsonStream = await GetOCRJson(pngSasLink);
        var jsonName = $"{folderName}/json/{i}.json";

        var jsonSasLink = await UploadAsync(jsonName, jsonStream);

        return new Links
        {
          PngUrl = pngSasLink,
          JsonUrl = jsonSasLink
        };
      }



      private static async Task<byte[]> DownloadAsync(string blobName)
      {
        var blobClient = new BlobClient(
          connectionString: "DefaultEndpointsProtocol=https;AccountName=sarumpolesearch;AccountKey=tYlgJ6zlz8GQCpIM32Ed4ta9NsOk5+M4Nf1B6gi53YJQzEua6jNYQBInFkmlMjj7mO8lLUZj/IoaVjlvCG6P9g==;EndpointSuffix=core.windows.net",
          blobContainerName: "rumpole",
          blobName: blobName);

        if (await blobClient.ExistsAsync())
        {
          using (var ms = new MemoryStream())
          {
            await blobClient.DownloadToAsync(ms);
            return ms.ToArray();
          }
        }
        return new byte[0];  // returns empty array
      }

      private static async Task<string> UploadAsync(string blobName, Stream stream)
      {
        var blobClient = new BlobClient(
          connectionString: "DefaultEndpointsProtocol=https;AccountName=sarumpolesearch;AccountKey=tYlgJ6zlz8GQCpIM32Ed4ta9NsOk5+M4Nf1B6gi53YJQzEua6jNYQBInFkmlMjj7mO8lLUZj/IoaVjlvCG6P9g==;EndpointSuffix=core.windows.net",
          blobContainerName: "rumpole",
          blobName: blobName);

        await blobClient.UploadAsync(stream, true);

        var sasBuilder = new BlobSasBuilder()
        {
          BlobContainerName = "rumpole",
          Resource = "c"
        };

        sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddDays(365 * 2);
        sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
        var fileSuffix = Path.GetExtension(blobName);
        sasBuilder.ContentType = fileSuffix.ToLower() == ".json" ? "application/json" : "image/png";

        return blobClient.GenerateSasUri(sasBuilder).AbsoluteUri;
      }

      private static async Task<Stream> GetOCRJson(string pngUrl)
      {
        var httpRequest = new HttpRequestMessage
        {
          Method = HttpMethod.Post,
          RequestUri = new Uri("https://cv-stef-search-service-paid.cognitiveservices.azure.com//vision/v3.2/read/analyze"),
        };
        httpRequest.Headers.Add("Ocp-Apim-Subscription-Key", "88f4fc58f07b4722a436cf3f0e926b95");
        httpRequest.Content = new StringContent("{\"url\":\"" + pngUrl + "\"}", Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(httpRequest);
        var continuationUrl = response.Headers.GetValues("Operation-Location").FirstOrDefault();

        while (true)
        {
          var continuationRequest = new HttpRequestMessage
          {
            Method = HttpMethod.Get,
            RequestUri = new Uri(continuationUrl),
          };
          continuationRequest.Headers.Add("Ocp-Apim-Subscription-Key", "88f4fc58f07b4722a436cf3f0e926b95");

          var continuationResponse = await httpClient.SendAsync(continuationRequest);

          var continuationResponseContent = await continuationResponse.Content.ReadAsStringAsync();
          dynamic continuationResponseJson = JObject.Parse(continuationResponseContent);

          if (continuationResponseJson.status == "succeeded")
          {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(continuationResponseContent);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
          }
        }

        // curl -v -X GET "https://westcentralus.api.cognitive.microsoft.com/vision/v3.2/read/analyzeResults/{operationId}" -H "Ocp-Apim-Subscription-Key: {subscription key}" --data-ascii "{body}" 
      }
    }
  }

  public class Links
  {
    public string PngUrl { get; set; }
    public string JsonUrl { get; set; }
  }

  public class ExampleFixture : IDisposable
  {
    public IDocLib DocNet { get; }

    public ExampleFixture()
    {
      DocNet = DocLib.Instance;
    }

    public void Dispose()
    {
      DocNet.Dispose();
    }
  }
*/
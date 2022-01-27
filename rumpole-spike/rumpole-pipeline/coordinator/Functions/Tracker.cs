using System.Runtime.CompilerServices;
using System.Globalization;
using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class Tracker : ITracker
{

    [JsonProperty("isIndexed")]
    public Boolean IsIndexed { get; set; }

    [JsonProperty("transactionId")]
    public string TransactionId { get; set; }

    [JsonProperty("documents")]
    public List<TrackerDocument> Documents { get; set; }

    [JsonProperty("logs")]
    public List<Log> Logs { get; set; }

    // a `Task` so we can wait for it and be assured we are initialsed and avoid null references
    public Task Initialise(string transactionId)
    {
        this.TransactionId = transactionId;
        this.Documents = new List<TrackerDocument>();
        this.Logs = new List<Log>();
        this.IsIndexed = false;

        Log(LogType.Initialise);

        return Task.CompletedTask;
    }

    // a `Task` so we can wait for it and be assured we are registered and avoid null references
    public Task Register(List<int> documentIds)
    {
        this.Documents = documentIds.Select(documentId => new TrackerDocument
        {
            DocumentId = documentId
        }).ToList();

        Log(LogType.Register);

        return Task.CompletedTask;
    }

    public void RegisterPdfUrl(TrackerPdfArg arg)
    {
        var doc = this.Documents.Find(document => document.DocumentId == arg.DocumentId);
        doc.PdfUrl = arg.PdfUrl;
        Log(LogType.RegisterPdfUrl, arg.DocumentId);
    }

    public void RegisterPngUrls(TrackerPngsArg arg)
    {
        var doc = this.Documents.Find(document => document.DocumentId == arg.DocumentId);
        doc.PngDetails = arg.PngUrls.Select(url => new TrackerPngDetails
        {
            Url = url
        }).ToList();
        Log(LogType.RegisterPngUrls, arg.DocumentId);
    }

    public void RegisterIsProcessedForSearch(TrackerSearchArg trackerSearchArg)
    {
        var doc = this.Documents.Find(document => document.DocumentId == trackerSearchArg.DocumentId);
        for (int i = 0; i < doc.PngDetails.Count; i++)
        {
            doc.PngDetails[i].Dimensions = trackerSearchArg.PngDimensions[i];
        }
        doc.IsProcessedForSearch = true;
        Log(LogType.RegisterIsIndexed, trackerSearchArg.DocumentId);
    }

    public void RegisterIsIndexed()
    {
        this.IsIndexed = true;
        Log(LogType.RegisterIsIndexed);
    }

    public Task<ITracker> Get()
    {
        return Task.FromResult((ITracker)this);
    }

    public Task<List<TrackerDocument>> GetDocuments()
    {
        return Task.FromResult(this.Documents);
    }

    private void Log(LogType logType, int? documentId = null)
    {
        this.Logs.Add(new Log
        {
            LogType = logType.ToString(),
            TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz"),
            DocumentId = documentId
        });
    }

    [FunctionName(nameof(Tracker))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx)
        => ctx.DispatchAsync<Tracker>();

    [FunctionName("Tracker_HttpStart")]
    public static async Task<IActionResult> HttpStart(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "cases/{caseId}/tracker")] HttpRequestMessage req,
        string caseId,
        [DurableClient] IDurableEntityClient client,
        ILogger log)
    {
        var entityId = new EntityId(nameof(Tracker), caseId);
        var stateResponse = await client.ReadEntityStateAsync<Tracker>(entityId);
        if (!stateResponse.EntityExists)
        {
            return (ActionResult)new NotFoundObjectResult("No entity with this id");
        }

        var response = await stateResponse.EntityState.Get();
        return (ActionResult)new OkObjectResult(response);
    }
}

public interface ITracker
{
    Task Initialise(string TransactionId);
    Task Register(List<int> documentIds);
    void RegisterPdfUrl(TrackerPdfArg arg);
    void RegisterPngUrls(TrackerPngsArg arg);
    void RegisterIsProcessedForSearch(TrackerSearchArg trackerSearchArg);
    void RegisterIsIndexed();
    Task<ITracker> Get();

    Task<List<TrackerDocument>> GetDocuments();
}

public class TrackerPngDimensions
{
    public double Height { get; set; }
    public double Width { get; set; }
}

public class TrackerPngDetails
{
    public string Url { get; set; }

    public TrackerPngDimensions Dimensions { get; set; }
}

public class TrackerDocument
{
    public int DocumentId { get; set; }

    public string PdfUrl { get; set; }

    public List<TrackerPngDetails> PngDetails { get; set; }

    public bool IsProcessedForSearch { get; set; }
}

public class TrackerPdfArg
{
    public int DocumentId { get; set; }

    public string PdfUrl { get; set; }
}

public class TrackerPngsArg
{
    public int DocumentId { get; set; }

    public List<string> PngUrls { get; set; }
}

public class TrackerSearchArg
{
    public int DocumentId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<TrackerPngDimensions> PngDimensions { get; set; }
}

public class Log
{
    public string LogType { get; set; }

    public string TimeStamp { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? DocumentId { get; set; }
}

public enum LogType
{
    Initialise,
    Register,
    RegisterPdfUrl,
    RegisterPngUrls,
    RegisterIsProcessedForSearch,
    RegisterIsIndexed
}
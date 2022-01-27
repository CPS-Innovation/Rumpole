using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace indexer_spike
{
    public static class CreateIndex
    {
        public static async Task Run(string[] args)
        {
            var httpClient = new HttpClient();

            var req = new HttpRequestMessage();
            var indexName = "lines-index";
            req.RequestUri = (new Uri($"https://ss-rumpole-spike.search.windows.net/indexes/{indexName}?api-version=2021-04-30-Preview"));
            req.Method = HttpMethod.Put;

            req.Content = new StringContent(File.ReadAllText("/Users/stef/code/CPS/Rumpole/rumpole-spike/indexer-spike/definition.json"));
            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            req.Headers.Add("api-key", "712AE73D012A53D7E818093C47A79B69");

            var response = await httpClient.SendAsync(req);
            Console.WriteLine("Run " + response.StatusCode);
        }
    }
}
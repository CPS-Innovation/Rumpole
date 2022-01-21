using System.Net.Http.Headers;
using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace indexer_spike
{
    class Program
    {
        static HttpClient _httpClient = new HttpClient();


        static async Task Main(string[] args)
        {
            await Run();

            while (true)
            {
                await Task.Delay(1000);
                await Status();
            }
        }

        static async Task Run()
        {
            var req = new HttpRequestMessage();
            req.RequestUri = (new Uri("https://ss-stef-search-service.search.windows.net/indexers/cosmosdb-indexer/run?api-version=2021-04-30-Preview"));
            req.Method = HttpMethod.Post;
            req.Content = new StringContent("");
            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            req.Headers.Add("api-key", "B0A3651B95A844E109780A56DE459C9B");

            var response = await _httpClient.SendAsync(req);
            Console.WriteLine("Run " + response.StatusCode);
        }

        static async Task Status()
        {
            var req = new HttpRequestMessage();
            req.RequestUri = (new Uri("https://ss-stef-search-service.search.windows.net/indexers/cosmosdb-indexer/status?api-version=2021-04-30-Preview"));

            req.Content = new StringContent("");
            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            req.Headers.Add("api-key", "B0A3651B95A844E109780A56DE459C9B");

            var response = await _httpClient.SendAsync(req);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}

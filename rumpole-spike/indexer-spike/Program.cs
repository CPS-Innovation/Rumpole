using System.Net.Http.Headers;
using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace indexer_spike
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await Indexer.Run(args);
            //await Client.Run(args);
            //await Client.GetCosmosRecords(124);
            await CreateIndex.Run(args);

        }
    }
}

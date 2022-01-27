
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.BlobStorageService;

using Services.PngService;

[assembly: FunctionsStartup(typeof(ServerlessPDFConversionDemo.Startup))]
namespace ServerlessPDFConversionDemo
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<BlobStorageOptions>().Configure<IConfiguration>((setttings, configuration) =>
            {
                configuration.GetSection("blobStorage").Bind(setttings);
            });

            builder.Services.AddSingleton<BlobStorageService>();
            builder.Services.AddSingleton<DocNetInstance>(); // must be a singleton https://github.com/GowenGit/docnet#usage
            builder.Services.AddSingleton<PngService>();
        }
    }
}
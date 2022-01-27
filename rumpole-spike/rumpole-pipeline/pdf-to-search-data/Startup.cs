
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.OcrService;
using Services.SearchDataStorageService;

[assembly: FunctionsStartup(typeof(ServerlessPDFConversionDemo.Startup))]
namespace ServerlessPDFConversionDemo
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddOptions<OcrOptions>().Configure<IConfiguration>((setttings, configuration) =>
            {
                configuration.GetSection("ocrService").Bind(setttings);
            });
            builder.Services.AddOptions<SearchDataStorageOptions>().Configure<IConfiguration>((setttings, configuration) =>
            {
                configuration.GetSection("searchDataStorage").Bind(setttings);
            });
            builder.Services.AddOptions<SearchDataIndexOptions>().Configure<IConfiguration>((setttings, configuration) =>
                {
                    configuration.GetSection("searchDataIndex").Bind(setttings);
                });
            builder.Services.AddSingleton<OcrService>();
            builder.Services.AddSingleton<SearchDataStorageService>();
        }
    }
}
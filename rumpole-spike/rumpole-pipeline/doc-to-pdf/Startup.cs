
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.BlobStorageService;
using Services.CmsService;
using Services.PdfService;


[assembly: FunctionsStartup(typeof(ServerlessPDFConversionDemo.Startup))]
namespace ServerlessPDFConversionDemo
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<AuthenticationOptions>().Configure<IConfiguration>((setttings, configuration) =>
            {
                configuration.GetSection("graph").Bind(setttings);
            });
            builder.Services.AddOptions<PdfOptions>().Configure<IConfiguration>((setttings, configuration) =>
            {
                configuration.GetSection("pdf").Bind(setttings);
            });
            builder.Services.AddOptions<BlobStorageOptions>().Configure<IConfiguration>((setttings, configuration) =>
            {
                configuration.GetSection("blobStorage").Bind(setttings);
            });

            builder.Services.AddSingleton<AuthenticationService>();
            builder.Services.AddSingleton<PdfService>();
            builder.Services.AddSingleton<BlobStorageService>();
            builder.Services.AddHttpClient<CmsService>();
        }
    }
}
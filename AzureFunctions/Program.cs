using Azure.Storage.Files.Shares;
using AzureFunctions.Infrastructure.ConvertApi;
using AzureFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddConvertApi(Environment.GetEnvironmentVariable("ConvertApiSecret"));
        services.AddScoped<FileService>();
        services.AddScoped<ConvertApiService>();
        services.AddScoped(provider => new ShareClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage"),
            Environment.GetEnvironmentVariable("AzureWebJobsStorage.PdfShare")));
        services.AddScoped(provider => provider
            .GetRequiredService<ShareClient>()
            .GetRootDirectoryClient());
    })
    .Build();

host.Run();

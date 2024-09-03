using Azure.Storage.Blobs;
using AzureFunctions.Infrastructure.ConvertApi;
using AzureFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddConvertApi(Environment.GetEnvironmentVariable("ConvertApiSecret"));
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        });
        services.AddScoped<BlobService>();
        services.AddScoped<ConvertApiService>();
        services.AddScoped(provider => provider.GetRequiredService<BlobServiceClient>()
            .GetBlobContainerClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage.Container.Data")));
    })
    .Build();

host.Run();

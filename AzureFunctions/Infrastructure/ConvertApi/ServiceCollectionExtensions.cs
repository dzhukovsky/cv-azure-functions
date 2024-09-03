using ConvertApiDotNet.Constants;
using ConvertApiDotNet.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFunctions.Infrastructure.ConvertApi;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConvertApi(this IServiceCollection services, string? apiSecret)
    {
        services.AddHttpClient<IConvertApiHttpClient, ConvertApiHttpClient>(client =>
        {
            client.Timeout = ConvertApiConstants.HttpClientTimeOut;
        });
        services.AddScoped(provider => new ConvertApiDotNet.ConvertApi(apiSecret, provider.GetRequiredService<IConvertApiHttpClient>()));
        return services;
    }
}

using ConvertApiDotNet.Interface;

namespace AzureFunctions.Infrastructure.ConvertApi;
public class ConvertApiHttpClient(HttpClient client) : IConvertApiHttpClient
{
    public HttpClient Client => client;
}

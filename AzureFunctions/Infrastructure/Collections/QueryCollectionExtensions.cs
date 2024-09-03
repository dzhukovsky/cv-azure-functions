using Microsoft.AspNetCore.Http;

namespace AzureFunctions.Infrastructure.Collections;
public static class QueryCollectionExtensions
{
    public static string? FirstOrDefault(this IQueryCollection queryCollection, string key)
        => queryCollection.TryGetValue(key, out var value) ? (string?)value : default;
}

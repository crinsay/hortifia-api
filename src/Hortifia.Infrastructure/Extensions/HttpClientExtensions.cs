using System.Net.Http.Json;
using System.Text.Json;

namespace Hortifia.Infrastructure.Extensions;

internal static class HttpClientExtensions
{
    public static async Task<object?> GetFromJsonOrDefaultAsync<TResponse>(this HttpClient httpClient, string requestUri, JsonSerializerOptions options)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<TResponse>(requestUri, options);
        }
        catch
        {
            return default;
        }
    }
}

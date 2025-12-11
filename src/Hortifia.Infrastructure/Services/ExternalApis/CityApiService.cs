using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Location.Responses;
using Hortifia.Infrastructure.Extensions;
using System.Globalization;
using System.Text.Json;

namespace Hortifia.Infrastructure.Services.ExternalApis;

internal class CityApiService(HttpClient httpClient) : ICityApiService
{
    public Task<string?> GetCityNameAsync(double latitude, double longitude)
    {
        var formattedLatitude = latitude.ToString(CultureInfo.InvariantCulture);
        var formattedLongitude = longitude.ToString(CultureInfo.InvariantCulture);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var cityRequestUrl = $@"?lat={formattedLatitude}&lon={formattedLongitude}&format=json";
        var cityTask = httpClient.GetFromJsonOrDefaultAsync<CityNameApiResponse>(cityRequestUrl, options);

        return cityTask.ContinueWith(task => task?.Result?.CityName);
    }
}

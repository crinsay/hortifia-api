using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Weather.Dtos;
using Hortifia.Application.Weather.Responses;
using Hortifia.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text.Json;

namespace Hortifia.Infrastructure.Services.ExternalApis;

internal class WeatherApiService(HttpClient httpClient, IConfiguration configuration) : IWeatherApiService
{
    public async Task<WeatherWithCityDto?> GetCurrentWeatherAsync(double latitude, double longitude)
    {
        var formattedLatitude = latitude.ToString(CultureInfo.InvariantCulture);
        var formattedLongitude = longitude.ToString(CultureInfo.InvariantCulture);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var weatherRequestUri = $@"?latitude={formattedLatitude}&longitude={formattedLongitude}&current=temperature_2m,weather_code&timezone=auto";
        var weatherTask = httpClient.GetFromJsonOrDefaultAsync<CurrentWeatherApiResponse>(weatherRequestUri, options);

        var cityRequestUrl = $@"{configuration["CityApi:BaseUrl"]}?lat={formattedLatitude}&lon={formattedLongitude}&format=json";
        var cityTask = httpClient.GetFromJsonOrDefaultAsync<CityNameApiResponse>(cityRequestUrl, options);

        var responses = await Task.WhenAll(weatherTask, cityTask);
        var weatherResponse = responses[0] as CurrentWeatherApiResponse;
        var cityResponse = responses[1] as CityNameApiResponse;

        return new WeatherWithCityDto
        {
            Temperature = weatherResponse?.CurrentWeather?.Temperature,
            Code = weatherResponse?.CurrentWeather?.Code,
            CityName = cityResponse?.Info?.Name
        };
    }

    public async Task<DailyWeatherInfoDto?> GetLongTermWeatherAsync(double latitude, double longitude, byte daysSpan = 7)
    {
        if (daysSpan > 16)
        {
            daysSpan = 7;
        }

        var formattedLatitude = latitude.ToString(CultureInfo.InvariantCulture);
        var formattedLongitude = longitude.ToString(CultureInfo.InvariantCulture);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var requestUri = $@"?latitude={formattedLatitude}&longitude={formattedLongitude}&daily=weather_code,temperature_2m_mean&timezone=auto&forecast_days={daysSpan}";
        var response = await httpClient.GetFromJsonOrDefaultAsync<DailyWeatherApiResponse>(requestUri, options) as DailyWeatherApiResponse;

        return response?.DailyWeather;
    }
}

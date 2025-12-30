using Hortifia.Application.Weather.Dtos;
using System.Text.Json.Serialization;

namespace Hortifia.Application.Weather.Responses;

public class CurrentWeatherApiResponse
{
    [JsonPropertyName("current")]
    public WeatherInfoDto? CurrentWeather { get; init; }

    [JsonPropertyName("daily")]
    public WeatherDailySunriseSunsetDto? Daily { get; init; }
}

using Hortifia.Application.Weather.Dtos;
using System.Text.Json.Serialization;

namespace Hortifia.Application.Weather.Responses;

public class DailyWeatherApiResponse
{
    [JsonPropertyName("daily")]
    public DailyWeatherInfoDto? DailyWeather { get; init; }
}

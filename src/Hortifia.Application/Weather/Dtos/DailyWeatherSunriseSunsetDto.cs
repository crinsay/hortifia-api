using System.Text.Json.Serialization;

namespace Hortifia.Application.Weather.Dtos;

public class WeatherDailySunriseSunsetDto
{
    [JsonPropertyName("sunrise")]
    public List<DateTime>? Sunrise { get; init; }

    [JsonPropertyName("sunset")]
    public List<DateTime>? Sunset { get; init; }
}
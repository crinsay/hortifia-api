using System.Text.Json.Serialization;

namespace Hortifia.Application.Weather.Dtos;

public class WeatherInfoDto
{
    [JsonPropertyName("temperature_2m")]
    public float? Temperature { get; init; }

    [JsonPropertyName("weather_code")]
    public int? Code { get; init; }
}

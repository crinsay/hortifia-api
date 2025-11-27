using System.Text.Json.Serialization;

namespace Hortifia.Application.Weather.Dtos;

public class WeatherWithCityDto
{
    public float? Temperature { get; init; }
    public int? Code { get; init; }
    public string? CityName { get; init; }
}

namespace Hortifia.Application.Weather.Dtos;

public class WeatherWithCityDto
{
    public float? Temperature { get; init; }
    public int? Code { get; init; }
    public string? CityName { get; init; }
    public DateTime? Sunrise { get; init; }
    public DateTime? Sunset { get; init; }
}

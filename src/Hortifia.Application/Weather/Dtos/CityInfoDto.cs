using System.Text.Json.Serialization;

namespace Hortifia.Application.Weather.Dtos;

public class CityInfoDto
{
    [JsonPropertyName("city")]
    public string? Name { get; init; }
}

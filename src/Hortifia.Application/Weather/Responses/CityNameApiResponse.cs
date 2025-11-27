using Hortifia.Application.Weather.Dtos;
using System.Text.Json.Serialization;

namespace Hortifia.Application.Weather.Responses;

public class CityNameApiResponse
{
    [JsonPropertyName("address")]
    public CityInfoDto? Info { get; init; }
}

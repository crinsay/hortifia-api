using System.Text.Json.Serialization;

namespace Hortifia.Application.Plants.Dtos;

public class PlantsApiResponseDto
{
    [JsonPropertyName("plants")]
    public List<PlantApiDto>? Plants { get; set; }
}

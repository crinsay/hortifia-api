using System.Text.Json.Serialization;

namespace Hortifia.Application.Plants.Dtos;

public class PlantsResponseDto
{
    [JsonPropertyName("plants")]
    public List<PlantDto>? Plants { get; set; }
}

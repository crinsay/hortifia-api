using System.Text.Json.Serialization;

namespace Hortifia.Application.Plants.Dtos;

public class PlantApiDto
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? CommonName { get; set; }

    [JsonPropertyName("scientific_name")]
    public string? ScientificName { get; set; }
    public string? Description { get; set; }
    public List<PlantApiDataDto>? Data { get; set; }
}

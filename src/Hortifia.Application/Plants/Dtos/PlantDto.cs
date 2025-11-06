using System.Text.Json.Serialization;

namespace Hortifia.Application.Plants.Dtos;

public class PlantDto
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? CommonName { get; set; }

    [JsonPropertyName("scientific_name")]
    public string? ScientificName { get; set; }
    public string? Description { get; set; }
    public List<PlantDataDto>? Data { get; set; }
    /*
     Plant data keys:
        - Edible
        - Growth
        - Water requirement
        - Light requirement
        - USDA Hardiness zone
        - Layer
        - Soil type
        - Family
        - Edible parts
     */
}

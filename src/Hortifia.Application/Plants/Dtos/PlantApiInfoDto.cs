namespace Hortifia.Application.Plants.Dtos;

public class PlantApiInfoDto
{
    public string? ScientificName { get; init; } = default!;
    public string? Family { get; init; } = default!;
    public string? Description { get; init; } = default!;
    public bool? IsEdible { get; init; }
    public string? Growth { get; init; }
    public string? WaterRequirement { get; init; }
    public string? LightRequirement { get; init; }
    public string? SoilType { get; init; }
    public string? EdibleParts { get; init; }
}

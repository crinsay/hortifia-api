namespace Hortifia.Application.Plants.Dtos;

public class PlantLookupDto
{
    public int PlantApiId { get; init; }
    public string CommonName { get; init; } = default!;
    public string ScientificName { get; init; } = default!;
}
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Plants.Dtos;

public class PlantListDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string CommonName { get; init; } = default!;
    public string? ImageBlobName { get; init; }
    public LightCondition LightCondition { get; init; } = LightCondition.Medium;
    public DateTime ExpectedWateringDate { get; init; }
    public bool IsFavourite { get; init; }
    public int PlantApiId { get; init; }
    public int RoomId { get; init; }
    public int WateringStatus { get; init; }
    public int DaysToNextWatering { get; init; }
    public bool IsInNeed { get; set; }
    }

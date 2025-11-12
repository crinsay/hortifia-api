namespace Hortifia.Application.Plants.Dtos;

public class PlantListDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string CommonName { get; init; } = default!;
    public string? ImageBlobName { get; init; }
    public DateTime ExpectedWateringDate { get; init; }
    public bool IsFavourite { get; init; }
    public int PlantApiId { get; init; }
    public int RoomId { get; init; }
    public int WateringStatus { get; set; }
    public int DaysToNextWatering { get; set; }
}

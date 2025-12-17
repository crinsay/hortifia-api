namespace Hortifia.Application.Plants.Dtos;

public class WateredPlantDto
{
    public int WateringStatus { get; init; }
    public DateTime LastWateringDate { get; init; }
    public DateTime ExpectedWateringDate { get; init; }
    public int DaysToNextWatering { get; init; }
    public bool IsWateringNeeded { get; init; }
}

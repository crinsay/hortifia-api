using Hortifia.Domain.Entities;

namespace Hortifia.Application.Plants.Dtos;

public class WateredPlantDto
{
    public int Id { get; init; }
    public int WateringStatus { get; init; }
    public DateTime LastWateringDate { get; init; }
    public DateTime ExpectedWateringDate { get; init; }
    public int DaysToNextWatering { get; init; }
    public bool IsWateringNeeded { get; init; }
    public bool IsOverexposedToSunlight { get; set; }

    public static WateredPlantDto CreateFromEntity(Plant plant)
    {
        var now = DateTime.UtcNow;

        return new WateredPlantDto
        {
            Id = plant.Id,
            ExpectedWateringDate = plant.ExpectedWateringDate,
            LastWateringDate = plant.LastWateringDate,
            WateringStatus = Math.Max((int)Math.Floor(
                    100 - (now - plant.LastWateringDate).TotalDays /
                    (plant.ExpectedWateringDate - plant.LastWateringDate).TotalDays * 100), 0),
            DaysToNextWatering = (int)Math.Ceiling(Math.Max((plant.ExpectedWateringDate - now).TotalDays, 0)),
            IsWateringNeeded = (Math.Max((int)Math.Floor(
                    100 - (now - plant.LastWateringDate).TotalDays /
                    (plant.ExpectedWateringDate - plant.LastWateringDate).TotalDays * 100), 0) < 20)
        };
    }
}

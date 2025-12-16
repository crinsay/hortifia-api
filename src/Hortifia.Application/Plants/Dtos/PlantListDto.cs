using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Plants.Dtos;

public class PlantListDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string CommonName { get; init; } = default!;
    public string? ImgUrl { get; set; }
    public LightCondition LightCondition { get; init; } = LightCondition.Medium;
    public DateTime ExpectedWateringDate { get; init; }
    public bool IsFavourite { get; init; }
    public int PlantApiId { get; init; }
    public int RoomId { get; init; }
    public int WateringStatus { get; init; }
    public int DaysToNextWatering { get; init; }
    public bool IsInNeed { get; set; }

    public static PlantListDto CreateFromEntity(Plant plant, float temperature)
    {
        var now = DateTime.UtcNow;

        return new PlantListDto
        {
            Id = plant.Id,
            Name = plant.Name,
            CommonName = plant.CommonName,
            ExpectedWateringDate = plant.ExpectedWateringDate,
            IsFavourite = plant.IsFavourite,
            PlantApiId = plant.PlantApiId,
            RoomId = plant.RoomId,
            WateringStatus = Math.Max((int)Math.Floor(
                        100 - (now - plant.LastWateringDate).TotalDays /
                        (plant.ExpectedWateringDate - plant.LastWateringDate).TotalDays * 100), 0),
            DaysToNextWatering = (int)Math.Ceiling(Math.Max((plant.ExpectedWateringDate - now).TotalDays, 0)),
            IsInNeed = (Math.Max((int)Math.Floor(
                        100 - (now - plant.LastWateringDate).TotalDays /
                              (plant.ExpectedWateringDate - plant.LastWateringDate).TotalDays * 100), 0) < 20)
                        || (plant.LightCondition == LightCondition.High && temperature > 30)
        };
    }
 }

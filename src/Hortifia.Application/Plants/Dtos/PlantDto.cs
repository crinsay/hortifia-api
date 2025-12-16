using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Plants.Dtos;

public class PlantDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string CommonName { get; init; } = default!;
    public string? ImgUrl { get; set; }
    public bool IsNearHeater { get; init; }
    public LightCondition LightCondition { get; init; } = LightCondition.Medium;
    public DateTime LastWateringDate { get; init; }
    public DateTime ExpectedWateringDate { get; init; }
    public bool IsFavourite { get; init; }
    public int PlantApiId { get; init; }
    public bool IsWateringNeeded { get; init; }
    public bool IsOverexposedToSunlight { get; set; }

    public RoomListDto Room { get; init; } = default!;
    public PlantApiInfoDto? PlantApiInfo { get; set; } = default!;

    public static PlantDto CreateFromEntity(Plant plant, float temperature)
    {
        return new PlantDto
        {
            Id = plant.Id,
            Name = plant.Name,
            CommonName = plant.CommonName,
            IsNearHeater = plant.IsNearHeater,
            LightCondition = plant.LightCondition,
            LastWateringDate = plant.LastWateringDate,
            ExpectedWateringDate = plant.ExpectedWateringDate,
            IsFavourite = plant.IsFavourite,
            PlantApiId = plant.PlantApiId,
            IsWateringNeeded = (Math.Max((int)Math.Floor(
                100 - (DateTime.UtcNow - plant.LastWateringDate).TotalDays /
                      (plant.ExpectedWateringDate - plant.LastWateringDate).TotalDays * 100), 0) < 20),
            IsOverexposedToSunlight = plant.LightCondition == LightCondition.High && temperature > 30,
            Room = RoomListDto.CreateFromEntity(plant.Room)
        };
    }
}

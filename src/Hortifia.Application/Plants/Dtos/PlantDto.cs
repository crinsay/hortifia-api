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

    public RoomListDto Room { get; init; } = default!;
    public PlantApiInfoDto? PlantApiInfo { get; set; } = default!;
}

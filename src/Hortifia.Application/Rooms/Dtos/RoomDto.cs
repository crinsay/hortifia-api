using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Rooms.Dtos;

public class RoomDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public RoomType Type { get; init; } = RoomType.Ordinary;
    public byte Humidity { get; init; }
    public float Temperature { get; init; }
    public string UserId { get; init; } = default!;

    //References
    public List<PlantListDto> Plants { get; init; } = [];
}

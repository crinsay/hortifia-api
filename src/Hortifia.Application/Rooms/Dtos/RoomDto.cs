using Hortifia.Application.Plants.Dtos;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Rooms.Dtos;

public class RoomDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public RoomType Type { get; init; } = RoomType.Ordinary;
    public byte Humidity { get; init; }
    public float Temperature { get; init; }

    //References
    public List<PlantListDto> Plants { get; init; } = [];

    public static RoomDto CreateFromEntity(Room room, float temperature)
    {
        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Type = room.Type,
            Humidity = room.Humidity,
            Temperature = room.Temperature,
            Plants = [.. room.Plants.Select(p => PlantListDto.CreateFromEntity(p, temperature))]
        };
    }
}

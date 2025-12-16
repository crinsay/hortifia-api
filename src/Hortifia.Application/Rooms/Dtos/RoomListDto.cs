using Hortifia.Application.Plants.Dtos;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Rooms.Dtos;

public class RoomListDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string UserId { get; init; } = default!;
    public List<string> PlantImgUrls { get; init; } = [];

    public static RoomListDto CreateFromEntity(Room room)
    {
        return new RoomListDto
        {
            Id = room.Id,
            Name = room.Name,
            UserId = room.OwnerId
        };
    }
}

using Hortifia.Domain.Entities;

namespace Hortifia.Application.Rooms.Dtos;

public class RoomDto
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public RoomType Type { get; private set; } = RoomType.Ordinary;
    public byte Humidity { get; private set; }
    public float Temperature { get; private set; }
    public string UserId { get; private set; } = default!;

    //References
    public List<Plant> Plants { get; set; } = [];
}

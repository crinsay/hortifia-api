using Hortifia.Domain.Entities;

namespace Hortifia.Application.Rooms.Commands.CreateRoom;

public class CreateRoomCommand
{
    public string Name { get; private set; } = default!;
    public RoomType Type { get; private set; } = RoomType.Ordinary;
    public byte Humidity { get; private set; }
    public float Temperature { get; private set; }
}

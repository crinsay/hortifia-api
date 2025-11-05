using Hortifia.Domain.Common;
using Hortifia.Domain.Entities;
using MediatR;

namespace Hortifia.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommand : IRequest<Result>
{
    public int RoomId { get; set; }
    public string Name { get; init; } = default!;
    public RoomType Type { get; init; } = RoomType.Ordinary;
    public byte Humidity { get; init; }
    public float Temperature { get; init; }
}

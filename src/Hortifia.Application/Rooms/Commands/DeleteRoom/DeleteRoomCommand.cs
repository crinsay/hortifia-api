using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Rooms.Commands.DeleteRoom;

public class DeleteRoomCommand : IRequest<Result>
{
    public int RoomId { get; init; }
}

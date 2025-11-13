using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Rooms.Commands.DeleteRoom;

public class DeleteRoomCommand : IRequest<Result>
{
    public required int RoomId { get; init; }
}

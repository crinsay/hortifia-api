using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Rooms.Queries.GetRoomById;

public class GetRoomByIdQuery : IRequest<Result<RoomDto>>
{
    public required int RoomId { get; init; }
}

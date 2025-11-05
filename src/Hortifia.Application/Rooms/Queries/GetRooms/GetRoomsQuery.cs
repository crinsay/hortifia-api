using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Rooms.Queries.GetRooms;

public class GetRoomsQuery : IRequest<Result<IEnumerable<RoomListDto>>>
{
    public string? SearchPhrase { get; init; }
}

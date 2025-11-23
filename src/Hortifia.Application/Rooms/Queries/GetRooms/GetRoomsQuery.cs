using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using MediatR;

namespace Hortifia.Application.Rooms.Queries.GetRooms;

public class GetRoomsQuery : IRequest<Result<IEnumerable<RoomListDto>>>
{
    public string? SearchPhrase { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool LimitToFour { get; init; } = false;
}

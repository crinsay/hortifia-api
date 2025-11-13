using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Rooms.Queries.GetRooms;

public class GetRoomsQueryHandler(IRoomsRepository roomsRepository,
    IUserContext userContext) : IRequestHandler<GetRoomsQuery, Result<IEnumerable<RoomListDto>>>
{
    public async Task<Result<IEnumerable<RoomListDto>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var rooms = await roomsRepository.GetAllDtosByUserIdAsync(currentUser.Id!, request.SearchPhrase);

        return Result<IEnumerable<RoomListDto>>.Success(rooms);
    }
}

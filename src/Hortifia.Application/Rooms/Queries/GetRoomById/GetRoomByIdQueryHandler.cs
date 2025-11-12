using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Rooms.Queries.GetRoomById;

public class GetRoomByIdQueryHandler(IRoomsRepository roomsRepository,
    ILogger<GetRoomByIdQueryHandler> logger,
    IUserContext userContext) : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
{
    public async Task<Result<RoomDto>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (!currentUser.IsAuthenticated || string.IsNullOrEmpty(currentUser.Id))
        {
            logger.LogWarning("Unauthorized attempt to create a room.");
            return Result<RoomDto>.Failure("User is not authenticated.");
        }

        var roomId = request.RoomId;
        var room = await roomsRepository.GetDtoByIdAsync(roomId);

        if (room is null)
        {
            logger.LogWarning("Room with id {roomId} not found", roomId);
            return Result<RoomDto>.Failure("Room doesn't exist");
        }

        if (room.UserId != currentUser.Id)
        {
            logger.LogWarning("User doesn't have access to this room");
            return Result<RoomDto>.Failure("User doesn't have access to this room");
        }

        return Result<RoomDto>.Success(room);
    }
}

using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using Hortifia.Domain.Entities;
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

        var roomId = request.RoomId;
        var room = await roomsRepository.GetDtoByIdAsync(roomId);

        if (room is null || room.UserId != currentUser.Id)
        {
            logger.LogWarning("Room with id {roomId} not found", roomId);
            return Result<RoomDto>.Failure("Room doesn't exist");
        }

        return Result<RoomDto>.Success(room);
    }
}

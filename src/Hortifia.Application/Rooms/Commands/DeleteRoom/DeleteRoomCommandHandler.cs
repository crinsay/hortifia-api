using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Rooms.Commands.DeleteRoom;

public class DeleteRoomCommandHandler(IRoomsRepository roomsRepository,
    ILogger<DeleteRoomCommandHandler> logger,
    IUserContext userContext) : IRequestHandler<DeleteRoomCommand, Result>
{
    public async Task<Result> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser.Id is null)
        {
            logger.LogWarning("Unauthorized attempt to delete room with ID {RoomId}", request.RoomId);
            return Result.Failure("User is not authenticated.");
        }

        var room = await roomsRepository.GetByIdAsync(request.RoomId);

        if (room is null)
        {
            logger.LogWarning("Room with ID {RoomId} not found for deletion", request.RoomId);
            return Result.Failure("Room not found.");
        }

        if (room.UserId != currentUser.Id)
        {
            logger.LogWarning("User {UserId} attempted to delete room with ID {RoomId} without permission", currentUser.Id, request.RoomId);
            return Result.Failure("User does not have permission to delete this room.");
        }

        await roomsRepository.DeleteAsync(room);

        return Result.Success();
    }
}

using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandHandler(IRoomsRepository roomsRepository,
    ILogger<UpdateRoomCommandHandler> logger,
    IUserContext userContext) : IRequestHandler<UpdateRoomCommand, Result>
{
    public async Task<Result> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var roomId = request.RoomId;

        if (currentUser.Id is null)
        {
            logger.LogWarning("Unauthorized attempt to update a room.");
            return Result.Failure("Unauthorized");
        }

        var roomToUpdate = await roomsRepository.GetByIdAsync(roomId);

        if (roomToUpdate is null)
        {
            logger.LogWarning("Room with ID {RoomId} not found.", roomId);
            return Result.Failure("Room not found");
        }

        if (roomToUpdate.UserId != currentUser.Id)
        {
            logger.LogWarning("User {UserId} attempted to update room {RoomId} which they do not own.", currentUser.Id, roomId);
            return Result.Failure("Forbidden");
        }

        roomToUpdate.Update(request.Name, request.Type, request.Humidity, request.Temperature);

        await roomsRepository.SaveChangesAsync();

        return Result.Success();
    }
}

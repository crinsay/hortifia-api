using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Rooms.Commands.DeleteRoom;

public class DeleteRoomCommandHandler(IRoomsRepository roomsRepository,
    IAuthorizationService authorizationService,
    ILogger<DeleteRoomCommandHandler> logger,
    IUserContext userContext) : IRequestHandler<DeleteRoomCommand, Result>
{
    public async Task<Result> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (!currentUser.IsAuthenticated || string.IsNullOrEmpty(currentUser.Id))
        {
            logger.LogWarning("Unauthorized attempt to delete a room.");
            return Result.Failure("User is not authenticated.");
        }

        var room = await roomsRepository.GetByIdAsync(request.RoomId);

        if (room is null)
        {
            logger.LogWarning("Room with ID {RoomId} not found for deletion", request.RoomId);
            return Result.Failure("Room not found.");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, room, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User {UserId} attempted to delete room with ID {RoomId} without permission", currentUser.Id, request.RoomId);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result<PostDto>.Failure("Room not found.");
        }

        await roomsRepository.DeleteAsync(room);

        return Result.Success();
    }
}

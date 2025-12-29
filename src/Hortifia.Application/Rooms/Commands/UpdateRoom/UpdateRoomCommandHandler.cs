using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Posts.Dtos;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandHandler(IRoomsRepository roomsRepository,
    IAuthorizationService authorizationService,
    ILogger<UpdateRoomCommandHandler> logger,
    IUserContext userContext) : IRequestHandler<UpdateRoomCommand, Result<RoomDto>>
{
    public async Task<Result<RoomDto>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        var roomId = request.RoomId;
        var roomToUpdate = await roomsRepository.GetByIdAsync(roomId);

        if (roomToUpdate is null)
        {
            logger.LogWarning("Room with ID {RoomId} not found.", roomId);
            return Result<RoomDto>.Failure("Room not found");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, roomToUpdate, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("User {UserId} attempted to update room {RoomId} which they do not own.", currentUser.Id, roomId);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result<RoomDto>.Failure("Room not found.");
        }

        roomToUpdate.Update(
            name: request.Name, 
            type: request.Type, 
            humidity: request.Humidity, 
            temperature: request.Temperature
            );

        await roomsRepository.SaveChangesAsync();

        return Result<RoomDto>.Success(RoomDto.CreateFromEntity(roomToUpdate, request.Temperature));
    }
}

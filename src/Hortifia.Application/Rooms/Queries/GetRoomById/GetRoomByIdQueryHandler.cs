using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using Hortifia.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Rooms.Queries.GetRoomById;

public class GetRoomByIdQueryHandler(IRoomsRepository roomsRepository,
    ILogger<GetRoomByIdQueryHandler> logger,
    IUserContext userContext,
    IIdentityRepository identityRepository,
    IWeatherApiService weatherApiService,
    IAuthorizationService authorizationService) : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
{
    public async Task<Result<RoomDto>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        var roomId = request.RoomId;

        var (latitude, longitude) = await identityRepository.GetUserCoordinatesAsync(currentUser.Id!);

        if (latitude is null || longitude is null)
        {
            return Result<RoomDto>.Failure("User coordinates not found - probably current user no longer exists.");
        }

        var weather = await weatherApiService.GetLongTermWeatherAsync(latitude.Value, longitude.Value, 1);

        if (weather is null)
        {
            return Result<RoomDto>.Failure("Unable to fetch current weather data - external APIs issue.");
        }

        if (weather.Temperatures is null)
        {
            return Result<RoomDto>.Failure("Incomplete weather data received from external APIs.");
        }

        var room = await roomsRepository.GetByIdAsync(roomId);

        if (room is null)
        {
            logger.LogWarning("Room with id {roomId} not found", roomId);
            return Result<RoomDto>.Failure("Room doesn't exist");
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(userContext.ClaimsPrincipalUser!, room, HortifiaPolicies.MustBeOwner);
        if (!authorizationResult.Succeeded)
        {
            logger.LogWarning("Room with id {roomId} not found", roomId);
            // We lie to the user that resource doesn't exist to prevent sensitive information leakage
            return Result<RoomDto>.Failure("Room not found.");
        }

        return Result<RoomDto>.Success(RoomDto.CreateFromEntity(room));
    }
}

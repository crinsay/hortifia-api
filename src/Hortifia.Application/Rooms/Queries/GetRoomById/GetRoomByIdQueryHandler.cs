using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Plants.Dtos;
using Hortifia.Application.Rooms.Dtos;
using Hortifia.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hortifia.Application.Rooms.Queries.GetRoomById;

public class GetRoomByIdQueryHandler(IRoomsRepository roomsRepository,
    ILogger<GetRoomByIdQueryHandler> logger,
    IUserContext userContext,
    IBlobStorageService blobStorageService,
    IIdentityRepository identityRepository,
    IWeatherApiService weatherApiService) : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
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

        var room = await roomsRepository.GetDtoByIdAsync(roomId, weather.Temperatures.First() ?? 20);

        if (room is null || room.UserId != currentUser.Id)
        {
            logger.LogWarning("Room with id {roomId} not found", roomId);
            return Result<RoomDto>.Failure("Room doesn't exist");
        }

        foreach (var plant in room.Plants)
        {
            var plantImgBlobName = plant.ImgUrl;
            if (plantImgBlobName is not null)
            {
                plant.ImgUrl = await blobStorageService.GetBlobSasUrlAsync(plantImgBlobName);
            }
        }

        return Result<RoomDto>.Success(room);
    }
}
